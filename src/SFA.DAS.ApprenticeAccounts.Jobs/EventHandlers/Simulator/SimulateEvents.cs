using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.Simulator
{
    internal class SimulateEvents
    {
        private readonly IFunctionEndpoint endpoint;

        public SimulateEvents(IFunctionEndpoint endpoint) => this.endpoint = endpoint;

        [FunctionName("ApprenticeshipUpdatedEmailAddressEventTrigger")]
        public async Task<IActionResult> ApprenticeshipUpdatedEmailAddressEvent(
            [HttpTrigger] HttpRequestMessage req, ExecutionContext executionContext, ILogger log)
        {
            try
            {
                var @event = new UpdateEmailAddressCommand
                {
                    ApprenticeId = Guid.NewGuid(),
                };

                var options = new SendOptions();
                options.RouteToThisEndpoint();

                await endpoint.Send(@event, options, executionContext, log);

                return new AcceptedResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

    }
}