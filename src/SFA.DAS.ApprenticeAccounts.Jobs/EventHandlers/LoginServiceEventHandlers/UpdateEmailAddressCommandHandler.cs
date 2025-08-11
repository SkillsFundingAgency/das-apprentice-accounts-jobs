using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.LoginServiceEventHandlers
{
    [ExcludeFromCodeCoverage]
    public class UpdateEmailAddressCommandHandler : IHandleMessages<UpdateEmailAddressCommand>
    {
        private readonly ILogger<UpdateEmailAddressCommandHandler> _logger;
        private readonly IOuterApiClient _outerApi;

        public UpdateEmailAddressCommandHandler(IOuterApiClient outerApi, ILogger<UpdateEmailAddressCommandHandler> logger)
            => (_outerApi, _logger) = (outerApi, logger);

        public Task Handle(UpdateEmailAddressCommand message, IMessageHandlerContext context)
        {
            string logMessage = $"Received UpdateEmailAddressCommand for apprentice {message.ApprenticeId}";
            _logger.LogInformation(logMessage);
            var requestBody = new JsonPatchDocument<Api.Apprentice>().Replace(x => x.Email, message.NewEmailAddress);

            return _outerApi.UpdateApprentice(message.ApprenticeId, requestBody);
        }
    }
}