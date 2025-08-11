﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class EsfaConfigurationExtension
    {
        public static void AddConfiguration(this IConfigurationBuilder builder)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true);

            var config = builder.Build();

            builder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = config["ConfigNames"].Split(",");
                options.StorageConnectionString = config["ConfigurationStorageConnectionString"];
                options.EnvironmentName = config["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });
        }

        public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
        {
            services
                .AddOptions<ApplicationSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.Bind(settings));
            services.AddSingleton(s => s.GetRequiredService<IOptions<ApplicationSettings>>().Value);

            return services;
        }

        public static IServiceCollection ConfigureFromOptions<TOptions>(this IServiceCollection services, Func<ApplicationSettings, TOptions> func)
            where TOptions : class, new()
        {
            services.AddSingleton(s =>
                func(s.GetRequiredService<IOptions<ApplicationSettings>>().Value));

            return services;
        }
    }
}
