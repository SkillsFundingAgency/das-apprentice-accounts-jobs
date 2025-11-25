using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    internal class AzureServiceBusTriggerFunction
    {
        private const string EndpointName = QueueNames.ApprenticeAccountsJobs;
        private readonly IFunctionEndpoint endpoint;

        public AzureServiceBusTriggerFunction(IFunctionEndpoint endpoint) => this.endpoint = endpoint;

        [FunctionName("ApprenticeAccountsJobs")]
        public async Task Run(
            [ServiceBusTrigger(queueName: EndpointName, Connection = "AzureWebJobsServiceBus")] ServiceBusReceivedMessage message,
            ILogger logger,
            ExecutionContext context)
        {
            await endpoint.ProcessNonAtomic(message, context, logger);
        }
    }
}