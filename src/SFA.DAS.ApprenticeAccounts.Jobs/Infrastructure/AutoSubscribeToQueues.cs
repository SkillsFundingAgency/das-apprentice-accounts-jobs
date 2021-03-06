using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus
{
    public static class AutoSubscribeToQueues
    {
        public static async Task CreateQueuesWithReflection(
            IConfiguration configuration,
            string connectionStringName = "AzureWebJobsServiceBus",
            string? errorQueue = null,
            string topicName = "bundle-1",
            ILogger? logger = null)
        {
            var connectionString = configuration.GetValue<string>(connectionStringName);
            var managementClient = new ManagementClient(connectionString);
            await CreateQueuesWithReflection(managementClient, errorQueue, topicName, logger);
        }

        public static async Task CreateQueuesWithReflection(
            ManagementClient managementClient,
            string? errorQueue = null,
            string topicName = "bundle-1",
            ILogger? logger = null)
        {
            var attribute = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttribute<FunctionNameAttribute>(false) != null)
                .SelectMany(m => m.GetParameters())
                .SelectMany(p => p.GetCustomAttributes<ServiceBusTriggerAttribute>(false))
                .FirstOrDefault()
                ?? throw new Exception("No endpoint was found");

            var endpointQueueName = attribute.QueueName;

            logger?.LogInformation("Queue Name: {queueName}", endpointQueueName);

            errorQueue ??= $"{endpointQueueName}-error";

            await CreateQueue(endpointQueueName, managementClient, logger);
            await CreateQueue(errorQueue, managementClient, logger);

            await CreateSubscription(topicName, managementClient, endpointQueueName, logger);
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
    }
}