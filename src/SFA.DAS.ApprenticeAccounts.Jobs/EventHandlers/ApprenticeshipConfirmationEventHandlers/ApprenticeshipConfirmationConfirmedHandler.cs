using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.ApprenticeshipConfirmationEventHandlers
{
    public class ApprenticeshipConfirmationConfirmedHandler : IHandleMessages<ApprenticeshipConfirmationConfirmedEvent>
    {
        private readonly ILogger<ApprenticeshipConfirmationConfirmedHandler> _logger;
        private readonly IOuterApiClient _outerApi;

        public ApprenticeshipConfirmationConfirmedHandler(IOuterApiClient outerApi, ILogger<ApprenticeshipConfirmationConfirmedHandler> logger)
            => (_outerApi, _logger) = (outerApi, logger);

        public Task Handle(ApprenticeshipConfirmationConfirmedEvent message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Received {nameof(ApprenticeshipConfirmationConfirmedEvent)} for apprentice {message.ApprenticeId}");

            return _outerApi.SendApprenticeshipConfirmed(message.ApprenticeId, new ApprenticeshipConfirmedRequest {
                CommitmentsApprenticeshipId = message.CommitmentsApprenticeshipId,
                ApprovedOn = message.CommitmentsApprovedOn
            });
        }
    }
}