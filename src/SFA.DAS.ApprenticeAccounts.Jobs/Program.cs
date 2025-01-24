using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.ApprenticeAccounts.Jobs.Configuration;
using SFA.DAS.ApprenticeAccounts.Jobs.Extensions;

[assembly: NServiceBusTriggerFunction(QueueNames.ApprenticeAccountsJobs)]
var host = new HostBuilder()

.ConfigureFunctionsWebApplication()
.ConfigureAppConfiguration(
    builder =>
    {
        builder.AddConfiguration();
    })
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
