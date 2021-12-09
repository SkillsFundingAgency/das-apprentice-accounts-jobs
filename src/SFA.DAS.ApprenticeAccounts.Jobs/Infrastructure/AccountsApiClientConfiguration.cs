using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    internal static class AccountsApiClientConfiguration
    {
        public static void AddAccountsApiClient(this IServiceCollection services)
        {
            services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

            services.AddHttpClient("AccountsApi")
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