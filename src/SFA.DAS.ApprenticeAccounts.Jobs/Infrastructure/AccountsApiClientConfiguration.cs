using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    internal static class AccountsApiClientConfiguration
    {

        public static IServiceCollection AddOuterApi(
            this IServiceCollection services,
            ApiOptions configuration)
        {
            services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

            global::NLog.LogManager.GetLogger("ServicesStartup").Info("ApiBaseUrl: {url}", configuration.ApiBaseUrl);

            services
                .AddRestEaseClient<IOuterApiClient>(configuration.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            services.AddTransient<IApimClientConfiguration>((_) => configuration);

            return services;
        }
    }
}