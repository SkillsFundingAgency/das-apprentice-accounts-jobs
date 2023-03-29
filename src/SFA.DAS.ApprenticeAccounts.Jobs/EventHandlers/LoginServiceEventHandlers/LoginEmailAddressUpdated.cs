using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.LoginServiceEventHandlers
{
    public class LoginEmailAddressUpdated : IHandleMessages<UpdateEmailAddressCommand>
    {
        private readonly ILogger<LoginEmailAddressUpdated> _logger;
        private readonly IOuterApiClient _outerApi;

        public LoginEmailAddressUpdated(IOuterApiClient outerApi, ILogger<LoginEmailAddressUpdated> logger)
            => (_outerApi, _logger) = (outerApi, logger);

        public Task Handle(UpdateEmailAddressCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Received {nameof(UpdateEmailAddressCommand)} for apprentice {message.ApprenticeId}");
            var requestBody = new JsonPatchDocument<Api.Apprentice>().Replace(x => x.Email, message.NewEmailAddress);

            return _outerApi.UpdateApprentice(message.ApprenticeId, requestBody);
        }
    }
}