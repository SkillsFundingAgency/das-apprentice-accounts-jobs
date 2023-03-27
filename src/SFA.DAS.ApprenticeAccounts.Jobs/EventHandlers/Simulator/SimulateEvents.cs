using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.CommitmentsV2.Messages.Events;
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

        [FunctionName("HandleApprenticeshipCreatedEventTrigger")]
        public Task<IActionResult> ApprenticeshipCreatedEvent(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "test-apprenticeship-created-event")] HttpRequestMessage req,
            ExecutionContext executionContext,
            ILogger log)
            => Simulate<ApprenticeshipCreatedEvent>(req, executionContext, log);

        [FunctionName("HandleApprenticeshipUpdatedEventTrigger")]
        public Task<IActionResult> ApprenticeshipUpdatedEvent(
            [HttpTrigger] HttpRequestMessage req, ExecutionContext executionContext, ILogger log)
            => Simulate<ApprenticeshipUpdatedApprovedEvent>(req, executionContext, log);

        [FunctionName("TestApprenticeshipStopped")]
        public Task<IActionResult> ApprenticeshipStoppedEvent(
                    [HttpTrigger] HttpRequestMessage req, ExecutionContext executionContext, ILogger log)
                    => Simulate<ApprenticeshipStoppedEvent>(req, executionContext, log);

        public async Task<IActionResult> Simulate<T>(HttpRequestMessage req, ExecutionContext executionContext, ILogger log)
        {
            try
            {
                var @event = JsonConvert.DeserializeObject<T>(await req.Content.ReadAsStringAsync());

                await endpoint.Publish(@event, executionContext, log);

                return new AcceptedResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

    }
}