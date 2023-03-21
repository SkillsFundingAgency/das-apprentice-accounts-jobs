using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using NServiceBus;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using SFA.DAS.Http.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Extensions;
using SFA.DAS.NServiceBus.Extensions;

[assembly: FunctionsStartup(typeof(SFA.DAS.ApprenticeAccounts.Jobs.Startup))]

namespace SFA.DAS.ApprenticeAccounts.Jobs
{
    public class Startup : FunctionsStartup
    {
        public IConfiguration Configuration { get; set; }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigureConfiguration();
        }
        
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Configuration = builder.GetContext().Configuration;
            var useManagedIdentity = !Configuration.IsLocalAcceptanceOrDev();

            builder.Services.AddLogging();
            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddApplicationOptions();
            builder.Services.ConfigureFromOptions(f => f.ApprenticeAccountsApi);
            builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<ApiOptions>());

            InitialiseNServiceBus();

            builder.UseNServiceBus((IConfiguration appConfiguration) =>
            {
                var configuration = ServiceBusEndpointFactory.CreateSingleQueueConfiguration(QueueNames.ApprenticeAccountsJobs, appConfiguration, useManagedIdentity);
                configuration.AdvancedConfiguration.UseNewtonsoftJsonSerializer();
                configuration.AdvancedConfiguration.UseMessageConventions();
                configuration.AdvancedConfiguration.EnableInstallers();
                return configuration;
            });

            builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<ApiOptions>());
            builder.Services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            builder.Services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            builder.Services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

            var appConfig = Configuration.Get<OuterApiConfiguration>();
            builder.Services.AddSingleton(appConfig);
            builder.Services.AddOuterApi(appConfig);

            var url = builder.Services
                .BuildServiceProvider()
                .GetRequiredService<ApiOptions>()
                .ApiBaseUrl;

            builder.Services.AddRestEaseClient<ApiOptions>(url)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();
        }

        public void InitialiseNServiceBus()
        {
            var m = new NServiceBusResourceManager(Configuration, !Configuration.IsLocalAcceptanceOrDev());
            m.CreateWorkAndErrorQueues(QueueNames.ApprenticeAccountsJobs).GetAwaiter().GetResult();
            m.SubscribeToTopicForQueue(typeof(Startup).Assembly, QueueNames.ApprenticeAccountsJobs).GetAwaiter().GetResult();
        }
    }
}