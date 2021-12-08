using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using SFA.DAS.Http.Configuration;
using SFA.DAS.NServiceBus.AzureFunctions.ServiceBus;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
[assembly: NServiceBusTriggerFunction(Startup.EndpointName)]

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    internal class Startup : FunctionsStartup
    {
        public const string EndpointName = "Bob";

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            //builder.ConfigureConfiguration();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            builder.UseAzureFunctionNServiceBus(EndpointName);

            builder.Services.AddApplicationOptions();
            builder.Services.ConfigureFromOptions(f => f.ApprenticeAccountsApi);
            builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<ApiOptions>());
            builder.Services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            builder.Services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            builder.Services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

            builder.Services.AddHttpClient("example")
                .ConfigureHttpClient((sp, client) =>
                {
                    var apiOptions = sp.GetRequiredService<ApiOptions>();
                    client.BaseAddress = new Uri(apiOptions.ApiBaseUrl);
                })
                .UseWithRestEaseClient<IApprenticeAccountsApi>()
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();
        }
    }
}