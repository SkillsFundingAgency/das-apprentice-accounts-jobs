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
        public static IServiceCollection AddInnerApi(this IServiceCollection services)
        {
            services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();

            var builder = services.AddHttpClient("AccountsApi")
                .ConfigureHttpClient((sp, client) =>
                {
                    var apiOptions = sp.GetRequiredService<ApiOptions>();
                    client.BaseAddress = new Uri(apiOptions.ApiBaseUrl);
                })
                .UseWithRestEaseClient<IApprenticeAccountsApi>()
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>();

            if (UseManagedIdentity())
            {
                services.AddTransient<IApimClientConfiguration, ApiOptions>();
                services.AddTransient<IManagedIdentityTokenGenerator, ManagedIdentityTokenGenerator>();
                services.AddTransient<Http.MessageHandlers.ManagedIdentityHeadersHandler>();
                
                builder.AddHttpMessageHandler<Http.MessageHandlers.ManagedIdentityHeadersHandler>();
            }
                
            builder.AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            return services;
        }

        private static bool UseManagedIdentity()
        {
            string environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ?? "";
            return !environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
        }
    }
}