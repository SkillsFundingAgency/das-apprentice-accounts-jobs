using Azure.Core;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NServiceBus;
using RestEase.Implementation;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Numerics;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.LoginServiceEventHandlers
{
    public class ApprenticeshipConfirmationConfirmed : IHandleMessages<ApprenticeshipConfirmationConfirmedEvent>
    {
        private readonly ILogger<ApprenticeshipConfirmationConfirmed> _logger;
        private readonly IOuterApiClient _outerApi;

        public ApprenticeshipConfirmationConfirmed(IOuterApiClient outerApi, ILogger<ApprenticeshipConfirmationConfirmed> logger)
            => (_outerApi, _logger) = (outerApi, logger);

        public Task Handle(ApprenticeshipConfirmationConfirmedEvent message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Received {nameof(ApprenticeshipConfirmationConfirmedEvent)} for apprentice {message.ApprenticeId}");

            return _outerApi.SendApprenticeshipConfirmed(message.ApprenticeId, new ApprenticeshipConfirmed() {
                CommitmentsApprenticeshipId = message.CommitmentsApprenticeshipId,
                ApprovedOn = message.CommitmentsApprovedOn
            });
        }
    }
}