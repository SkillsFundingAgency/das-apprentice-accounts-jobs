using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SFA.DAS.NServiceBus.AzureFunctions.ServiceBus
{
    public static class ConfigureNServiceBusExtension
    {
        public static void ConfigureServiceBusManagedIdentity(
            this IFunctionsConfigurationBuilder builder,
            string connectionStringName = UseNServiceBusExtension.DefaultConnectionStringName)
        {
            var preConfig = builder.ConfigurationBuilder.Build();

            var key = $"{connectionStringName}__fullyQualifiedNamespace";
            var serviceBusNamespace = preConfig.GetValue<string>(key);
            if (serviceBusNamespace != null)
            {
                builder.ConfigurationBuilder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {
                            connectionStringName,
                            $"Endpoint=sb://{serviceBusNamespace}/;Authentication=Managed Identity;"
                        }
                    });
            }
        }
    }
}