﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.IO;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    internal static class EsfaConfigurationExtension
    {
        internal static void ConfigureConfiguration(this IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory());
                

            var preConfig = builder.ConfigurationBuilder.Build();

            if (!preConfig.IsAcceptanceOrDev())
            {
                builder.ConfigurationBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = preConfig["ConfigNames"].Split(",");
                    options.StorageConnectionString = preConfig["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = preConfig["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                });
            }
            builder.ConfigurationBuilder.AddJsonFile("local.settings.json", optional: true);
        }

        public static void AddApplicationOptions(this IServiceCollection services)
        {
            services
                .AddOptions<ApplicationSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.Bind(settings));
            services.AddSingleton(s => s.GetRequiredService<IOptions<ApplicationSettings>>().Value);
        }

        public static void ConfigureFromOptions<TOptions>(this IServiceCollection services, Func<ApplicationSettings, TOptions> func)
            where TOptions : class, new()
        {
            services.AddSingleton(s =>
                func(s.GetRequiredService<IOptions<ApplicationSettings>>().Value));
        }
    }
}