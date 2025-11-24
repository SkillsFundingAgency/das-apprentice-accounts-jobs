using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
[assembly: NServiceBusTriggerFunction(QueueNames.ApprenticeAccountsJobs)]

namespace SFA.DAS.ApprenticeAccounts.Jobs
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureAppConfiguration(builder => builder.AddConfiguration())
                .ConfigureNServiceBus(QueueNames.ApprenticeAccountsJobs)
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddApplicationInsightsTelemetryWorkerService()
                        .ConfigureFunctionsApplicationInsights()
                        .AddApplicationOptions()
                        .ConfigureFromOptions(f => f.ApprenticePortalOuterApi)
                        .AddServiceRegistrations();
                })
                .Build();
            await host.RunAsync();
        }
    }
}