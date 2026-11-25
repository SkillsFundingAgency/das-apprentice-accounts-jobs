using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.Transport.AzureServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class AddNServiceBusExtension
    {
        public static IHostBuilder ConfigureNServiceBus(this IHostBuilder hostBuilder, string endpointName)
        {
            hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
            {
                endpointConfiguration.AdvancedConfiguration.EnableInstallers();
                endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo($"{endpointName}-error");
                endpointConfiguration.AdvancedConfiguration.Conventions()
                    .DefiningCommandsAs(IsCommand)
                    .DefiningEventsAs(IsEvent)
                    .DefiningMessagesAs(IsMessage);

                var value = config["NServiceBusLicense"];
                if (!string.IsNullOrEmpty(value))
                {
                    var decodedLicence = WebUtility.HtmlDecode(value);
                    endpointConfiguration.AdvancedConfiguration.License(decodedLicence);
                }

                endpointConfiguration.Routing.RouteToEndpoint(typeof(UpdateEmailAddressCommand), QueueNames.ApprenticeAccountsJobs);

                UseLegacySingleTopic(endpointConfiguration.Transport);
            });

            return hostBuilder;
        }

        /// <summary>
        /// Transport 5+ defaults to one topic per event. The publishers of the events we handle
        /// (NServiceBus 7) still publish everything to the single "bundle-1" topic, so subscribe there
        /// using the same SQL filter on the NServiceBus.EnclosedMessageTypes header NServiceBus 7 used.
        /// </summary>
        internal static void UseLegacySingleTopic(AzureServiceBusTransport transport)
        {
            if (transport.Topology is not TopicPerEventTopology topology)
            {
                throw new InvalidOperationException(
                    $"Expected the topic-per-event topology but found {transport.Topology.GetType().Name}. " +
                    "Remove any AzureServiceBus:MigrationTopologyOptions app settings.");
            }

            topology.UseFallbackTopic(LegacyTopicName, TopicRoutingMode.SqlLikeFilter);
        }

        internal const string LegacyTopicName = "bundle-1";

        internal static bool IsMessage(Type t) => t is IMessage || IsDasMessage(t, "Messages");

        internal static bool IsEvent(Type t) => t is IEvent || IsDasMessage(t, "Messages.Events");

        internal static bool IsCommand(Type t) => t is ICommand || IsDasMessage(t, "Messages.Commands");

        private static bool IsDasMessage(Type t, string namespaceSuffix)
            => t.Namespace != null &&
               t.Namespace.StartsWith("SFA.DAS") &&
               t.Namespace.EndsWith(namespaceSuffix);
    }
}