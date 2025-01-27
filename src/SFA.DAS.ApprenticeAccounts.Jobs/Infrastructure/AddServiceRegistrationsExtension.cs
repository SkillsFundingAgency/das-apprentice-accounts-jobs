using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    public static class AddServiceRegistrationsExtension
    {
        public static IServiceCollection AddServiceRegistrations(this IServiceCollection services)
        {
            services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<ApiOptions>());
            services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

            var appConfig = services.BuildServiceProvider().GetRequiredService<ApiOptions>();
            services.AddSingleton(appConfig);
            services.AddOuterApi(appConfig);

            services.AddRestEaseClient<ApiOptions>(appConfig.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            return services;
        }
    }
}
