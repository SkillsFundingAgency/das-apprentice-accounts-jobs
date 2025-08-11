using Microsoft.Extensions.Hosting;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
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

            });

            return hostBuilder;
        }

        private static bool IsMessage(Type t) => t is IMessage || IsDasMessage(t, "Messages");

        private static bool IsEvent(Type t) => t is IEvent || IsDasMessage(t, "Messages.Events");

        private static bool IsCommand(Type t) => t is ICommand || IsDasMessage(t, "Messages.Commands");

        private static bool IsDasMessage(Type t, string namespaceSuffix)
            => t.Namespace != null &&
               t.Namespace.StartsWith("SFA.DAS") &&
               t.Namespace.EndsWith(namespaceSuffix);
    }
}