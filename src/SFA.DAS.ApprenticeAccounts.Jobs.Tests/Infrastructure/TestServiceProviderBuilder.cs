using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Tests.Infrastructure
{
    /// <summary>
    /// Builds the same service registrations as Program.cs (minus the Functions host and
    /// NServiceBus, which need real Azure resources), with the network swapped for a recorder.
    /// </summary>
    public static class TestServiceProviderBuilder
    {
        public const string ApiBaseUrl = "https://outer-api.test/";
        public const string SubscriptionKey = "test-subscription-key";
        public const string ApiVersion = "1";

        public static IServiceProvider Build(RecordingHttpMessageHandler? network = null)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ApprenticePortalOuterApi:ApiBaseUrl"] = ApiBaseUrl,
                    ["ApprenticePortalOuterApi:SubscriptionKey"] = SubscriptionKey,
                    ["ApprenticePortalOuterApi:ApiVersion"] = ApiVersion,
                    ["ApprenticePortalOuterApi:IdentifierUri"] = "",
                })
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddLogging();

            // Same chain as Program.cs
            services
                .AddApplicationOptions()
                .ConfigureFromOptions(f => f.ApprenticePortalOuterApi)
                .AddServiceRegistrations();

            if (network != null)
            {
                services.ConfigureAll<HttpClientFactoryOptions>(options =>
                    options.HttpMessageHandlerBuilderActions.Add(b => b.PrimaryHandler = network));
            }

            return services.BuildServiceProvider();
        }
    }
}
