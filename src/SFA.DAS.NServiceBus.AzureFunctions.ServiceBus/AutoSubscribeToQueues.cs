using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.AzureFunctions.ServiceBus
{
    public static class AutoSubscribeToQueues
    {
        public static async Task CreateQueues(
            Assembly assemblyWithTriggerAttribute,
            IConfiguration configuration,
            string connectionStringName = "AzureWebJobsServiceBus",
            string? errorQueue = null,
            string topicName = "bundle-1",
            ILogger? logger = null)
        {
            var connectionString = configuration.GetValue<string>(connectionStringName);
            var managementClient = new ManagementClient(connectionString);
            await CreateQueuesWithReflection(assemblyWithTriggerAttribute, managementClient, errorQueue, topicName, logger);
        }

        public static async Task CreateQueuesWithReflection(
            Assembly assemblyWithTriggerAttribute,
            ManagementClient managementClient,
            string? errorQueue = null,
            string topicName = "bundle-1",
            ILogger? logger = null)
        {
            var endpointQueueName = FindEndpointQueueName(assemblyWithTriggerAttribute);

            logger?.LogInformation("Queue Name: {queueName}", endpointQueueName);

            errorQueue ??= $"{endpointQueueName}-error";

            await CreateQueue(endpointQueueName, managementClient, logger);
            await CreateQueue(errorQueue, managementClient, logger);

            await CreateSubscription(topicName, managementClient, endpointQueueName, logger);
        }

        private static string FindEndpointQueueName(Assembly assemblyWithTriggerAttribute)
        {
            var triggerAttribute = assemblyWithTriggerAttribute
                .GetCustomAttributes<NServiceBusTriggerFunctionAttribute>(inherit: false)
                .FirstOrDefault();

            if(triggerAttribute == null)
            {
                throw new Exception(
                    $"No [NServiceBusTriggerFunctionAttribute] attribute was found in {assemblyWithTriggerAttribute.FullName}");
            }

            return triggerAttribute.EndpointName;
        }

        private static async Task CreateQueue(string endpointQueueName, ManagementClient managementClient, ILogger? logger)
        {
            if (await managementClient.QueueExistsAsync(endpointQueueName)) return;

            logger?.LogInformation("Creating queue: `{queueName}`", endpointQueueName);
            await managementClient.CreateQueueAsync(endpointQueueName);
        }

        private static async Task CreateSubscription(string topicName, ManagementClient managementClient, string endpointQueueName, ILogger? logger)
        {
            if (await managementClient.SubscriptionExistsAsync(topicName, endpointQueueName)) return;

            logger?.LogInformation($"Creating subscription to: `{endpointQueueName}`", endpointQueueName);

            var description = new SubscriptionDescription(topicName, endpointQueueName)
            {
                ForwardTo = endpointQueueName,
                UserMetadata = $"Subscribed to {endpointQueueName}"
            };

            var ignoreAllEvents = new RuleDescription { Filter = new FalseFilter() };

            await managementClient.CreateSubscriptionAsync(description, ignoreAllEvents);
        }

        public static IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }
            }
            while (stack.Count > 0);
        }
    }

    public static class ReflectionExtensions
    {
        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly assembly, bool inherit)
            => assembly.GetCustomAttributes(typeof(T), inherit).Cast<T>();
    }
}