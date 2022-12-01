using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.IO;
using System.Reflection;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    internal static class EsfaConfigurationExtension
    {
        internal static void ConfigureConfiguration(this IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddJsonFile("local.settings.json", optional: true);

            var preConfig = builder.ConfigurationBuilder.Build();

            builder.ConfigurationBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = preConfig["ConfigNames"].Split(",");
                options.StorageConnectionString = preConfig["ConfigurationStorageConnectionString"];
                options.EnvironmentName = preConfig["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });
        }

        public static void AddApplicationOptions(this IServiceCollection services)
        {
            services
                .AddOptions<ApplicationSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.Bind(settings));
            services.AddSingleton(s => s.GetRequiredService<IOptions<ApplicationSettings>>().Value);
        }

        public static void ConfigureLogging(this ILoggingBuilder logBuilder)
        {
            logBuilder.AddFilter(typeof(Startup).Namespace, LogLevel.Information);
            var rootDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, ".."));
            var files = Directory.GetFiles(rootDirectory, "nlog.config", SearchOption.AllDirectories)[0];
            logBuilder.AddNLog(files);
        }

        public static void ConfigureFromOptions<TOptions>(this IServiceCollection services, Func<ApplicationSettings, TOptions> func)
            where TOptions : class, new()
        {
            services.AddSingleton(s =>
                func(s.GetRequiredService<IOptions<ApplicationSettings>>().Value));
        }
    }
}