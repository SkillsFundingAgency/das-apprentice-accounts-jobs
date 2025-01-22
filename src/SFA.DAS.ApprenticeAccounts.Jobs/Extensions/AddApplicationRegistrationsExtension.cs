using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using SFA.DAS.Http.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class AddApplicationRegistrationsExtension
    {
        public static IServiceCollection AddApplicationRegistrations(this IServiceCollection services)
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
