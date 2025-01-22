using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class AddNServiceBusExtension
    {
        public const string EndpointName = "SFA.DAS.ApprenticesAccounts.Jobs";

        public static IHostBuilder ConfigureNServiceBus(this IHostBuilder hostBuilder, string endpointName)
        {
            hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
            {
                endpointConfiguration.AdvancedConfiguration.EnableInstallers();
                endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo($"{endpointName}-error");
                
                var value = config["NServiceBusLicense"];
                if (!string.IsNullOrEmpty(value))
                {
                    var decodedLicence = WebUtility.HtmlDecode(value);
                    endpointConfiguration.AdvancedConfiguration.License(decodedLicence);
                }

#if DEBUG
                var transport = endpointConfiguration.AdvancedConfiguration.UseTransport<LearningTransport>();
                transport.StorageDirectory(Path.Combine(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
                    @"src\.learningtransport"));
#endif
                endpointConfiguration.Routing.RouteToEndpoint(typeof(UpdateEmailAddressCommand), QueueNames.NotificationsQueue);
            });

            return hostBuilder;
        }
    }
}
