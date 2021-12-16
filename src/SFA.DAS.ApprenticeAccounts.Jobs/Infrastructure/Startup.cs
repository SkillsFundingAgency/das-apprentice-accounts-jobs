using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using SFA.DAS.NServiceBus.AzureFunctions.ServiceBus;

[assembly: FunctionsStartup(typeof(Startup))]
[assembly: NServiceBusTriggerFunction(Startup.EndpointName)]

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    internal class Startup : FunctionsStartup
    {
        public const string EndpointName = "SFA-DAS-ApprenticeAccounts";

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigureConfiguration();
            builder.ConfigureServiceBusManagedIdentity();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.ConfigureLogging();

            builder.UseAzureFunctionNServiceBus(EndpointName);

            builder.Services.AddApplicationOptions();
            builder.Services.ConfigureFromOptions(f => f.ApprenticeAccountsApi);
            builder.Services.AddInnerApi();
        }
    }
}