using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using System;
using System.Collections.Generic;

[assembly: FunctionsStartup(typeof(SFA.DAS.ApprenticeAccounts.Jobs.Startup))]

namespace SFA.DAS.ApprenticeAccounts.Jobs
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigureConfiguration();
            builder.ConfigureServiceBusManagedIdentity();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.ConfigureLogging();

            var logger = LoggerFactory.Create(b => b.ConfigureLogging()).CreateLogger<Startup>();

            AutoSubscribeToQueues.CreateQueuesWithReflection(
                builder.GetContext().Configuration,
                connectionStringName: "AzureWebJobsServiceBus",
                logger: logger)
                .GetAwaiter().GetResult();

            builder.UseNServiceBus((IConfiguration appConfiguration) =>
            {
                var configuration = new ServiceBusTriggeredEndpointConfiguration(
                    endpointName: QueueNames.ApprenticeshipCommitmentsJobs,
                    configuration: appConfiguration);

                configuration.AdvancedConfiguration.SendFailedMessagesTo($"{QueueNames.ApprenticeshipCommitmentsJobs}-error");
                configuration.LogDiagnostics();

                configuration.AdvancedConfiguration.Conventions()
                    .DefiningMessagesAs(IsMessage)
                    .DefiningEventsAs(IsEvent)
                    .DefiningCommandsAs(IsCommand);

                configuration.Transport.SubscriptionRuleNamingConvention(AzureQueueNameShortener.Shorten);

                configuration.Transport.Routing().RouteToEndpoint(typeof(SendEmailCommand), QueueNames.NotificationsQueue);

                configuration.AdvancedConfiguration.Pipeline.Register(new LogIncomingBehaviour(), nameof(LogIncomingBehaviour));
                configuration.AdvancedConfiguration.Pipeline.Register(new LogOutgoingBehaviour(), nameof(LogOutgoingBehaviour));

                var persistence = configuration.AdvancedConfiguration.UsePersistence<AzureTablePersistence>();
                persistence.ConnectionString(appConfiguration.GetConnectionStringOrSetting("AzureWebJobsStorage"));

                configuration.AdvancedConfiguration.EnableInstallers();

                return configuration;
            });

            builder.Services.AddApplicationOptions();
            builder.Services.ConfigureFromOptions(f => f.ApprenticeAccountsApi);
            builder.Services.AddInnerApi();
        }

        private static bool IsMessage(Type t) => t is IMessage || IsSfaMessage(t, "Messages");

        private static bool IsEvent(Type t) => t is IEvent || IsSfaMessage(t, "Messages.Events");

        private static bool IsCommand(Type t) => t is ICommand || IsSfaMessage(t, "Messages.Commands");

        private static bool IsSfaMessage(Type t, string namespaceSuffix)
            => t.Namespace != null &&
                t.Namespace.StartsWith("SFA.DAS") &&
                t.Namespace.EndsWith(namespaceSuffix);
    }

    public static class ConfigureNServiceBusExtension
    {
        public static void ConfigureServiceBusManagedIdentity(
            this IFunctionsConfigurationBuilder builder,
            string connectionStringName = "AzureWebJobsServiceBus")
        {
            var preConfig = builder.ConfigurationBuilder.Build();

            var key = $"{connectionStringName}__fullyQualifiedNamespace";
            var serviceBusNamespace = preConfig.GetValue<string>(key);
            if (serviceBusNamespace != null)
            {
                builder.ConfigurationBuilder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {
                            connectionStringName,
                            $"Endpoint=sb://{serviceBusNamespace}/;Authentication=Managed Identity;"
                        }
                    });
            }
        }
    }
}