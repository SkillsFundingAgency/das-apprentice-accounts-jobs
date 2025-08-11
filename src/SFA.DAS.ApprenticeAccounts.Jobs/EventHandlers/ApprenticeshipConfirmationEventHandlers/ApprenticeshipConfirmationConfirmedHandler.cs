using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.ApprenticeshipConfirmationEventHandlers
{
    [ExcludeFromCodeCoverage]
    public class ApprenticeshipConfirmationConfirmedHandler : IHandleMessages<ApprenticeshipConfirmationConfirmedEvent>
    {
        private readonly ILogger<ApprenticeshipConfirmationConfirmedHandler> _logger;
        private readonly IOuterApiClient _outerApi;

        public ApprenticeshipConfirmationConfirmedHandler(IOuterApiClient outerApi, ILogger<ApprenticeshipConfirmationConfirmedHandler> logger)
            => (_outerApi, _logger) = (outerApi, logger);

        public Task Handle(ApprenticeshipConfirmationConfirmedEvent message, IMessageHandlerContext context)
        {
            string logMessage = $"Received ApprenticeshipConfirmationConfirmedEvent for apprentice {message.ApprenticeId}";
            _logger.LogInformation(logMessage);

            return _outerApi.SendApprenticeshipConfirmed(message.ApprenticeId, new ApprenticeshipConfirmedRequest {
                CommitmentsApprenticeshipId = message.CommitmentsApprenticeshipId,
                ApprovedOn = message.CommitmentsApprovedOn
            });
        }
    }
}