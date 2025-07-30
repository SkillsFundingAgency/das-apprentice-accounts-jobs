using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
[assembly: NServiceBusTriggerFunction(QueueNames.ApprenticeAccountsJobs)]

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
