using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker;
using SFA.DAS.ApprenticeAccounts.Jobs.Extensions;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Hosting;

//[assembly: NServiceBusTriggerFunction(QueueNames.ApprenticeAccountsJobs)]

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

        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });
        services.AddApplicationOptions();
        services.AddApplicationRegistrations();
        services.AddApplicationInsightsTelemetryWorkerService()
        .ConfigureFunctionsApplicationInsights();
    })
    .Build();
await host.RunAsync();


