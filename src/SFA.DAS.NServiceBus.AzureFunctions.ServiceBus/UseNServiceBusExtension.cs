using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NServiceBus;
using System;

namespace SFA.DAS.NServiceBus.AzureFunctions.ServiceBus
{
    public static class UseNServiceBusExtension
    {
        public const string DefaultConnectionStringName = "AzureWebJobsServiceBus";

        public static void UseAzureFunctionNServiceBus(
            this IFunctionsHostBuilder builder,
            string endpointName,
            string connectionStringName = DefaultConnectionStringName)
        {
            builder.UseAzureFunctionNServiceBus(
                endpointName, connectionStringName, config => { });
        }

        public static void UseAzureFunctionNServiceBus(
            this IFunctionsHostBuilder builder,
            string endpointName,
            Action<ServiceBusTriggeredEndpointConfiguration> configurationFactory)
        {
            builder.UseAzureFunctionNServiceBus(
                endpointName, DefaultConnectionStringName, configurationFactory);
        }

        public static void UseAzureFunctionNServiceBus(
            this IFunctionsHostBuilder builder,
            string endpointName,
            string connectionStringName,
            Action<ServiceBusTriggeredEndpointConfiguration> configurationFactory)
        {
            builder.UseNServiceBus(hostConfig =>
            {
                var configuration = new ServiceBusTriggeredEndpointConfiguration(
                    endpointName: endpointName);

                var managedIdentityConnectionStringName = $"{connectionStringName}__fullyQualifiedNamespace";
                var managedIdentityConnectionString = hostConfig[managedIdentityConnectionStringName]
                ?? throw new Exception(
                    $"Azure Service Bus connection string namespace has not been configured. " +
                    $"Specify a connection string through an IConfiguration property named {managedIdentityConnectionStringName}.");

                configuration.Transport.ConnectionString(managedIdentityConnectionString);
                configuration.Transport.CustomTokenCredential(new DefaultAzureCredential());

                configuration.LogDiagnostics();
                configuration.DefineConventions();
                configuration.AdvancedConfiguration.SendFailedMessagesTo($"{endpointName}-error");
                configuration.Transport.SubscriptionRuleNamingConvention(
                    AzureQueueNameShortener.Shorten);

                configurationFactory(configuration);

                return configuration;
            });
        }

        public static ServiceBusTriggeredEndpointConfiguration DefineConventions(
            this ServiceBusTriggeredEndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.AdvancedConfiguration.Conventions()
                .DefiningMessagesAs(IsMessage)
                .DefiningEventsAs(IsEvent)
                .DefiningCommandsAs(IsCommand);
            return endpointConfiguration;
        }

        private static bool IsMessage(Type t) => t is IMessage || IsSfaMessage(t, "Messages");

        private static bool IsEvent(Type t) => t is IEvent || IsSfaMessage(t, "Messages.Events");

        private static bool IsCommand(Type t) => t is ICommand || IsSfaMessage(t, "Messages.Commands");

        private static bool IsSfaMessage(Type t, string namespaceSuffix)
            => t.Namespace != null &&
                t.Namespace.StartsWith("SFA.DAS") &&
                t.Namespace.EndsWith(namespaceSuffix);
    }
}