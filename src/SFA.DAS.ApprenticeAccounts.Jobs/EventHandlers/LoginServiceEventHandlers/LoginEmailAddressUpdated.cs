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
        private readonly IApprenticeAccountsApi _api;
        private readonly ILogger<LoginEmailAddressUpdated> _logger;

        public LoginEmailAddressUpdated(IApprenticeAccountsApi api, ILogger<LoginEmailAddressUpdated> logger)
            => (_api, _logger) = (api, logger);

        public Task Handle(UpdateEmailAddressCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Received {nameof(UpdateEmailAddressCommand)} for apprentice {message.ApprenticeId}");
            var requestBody = new JsonPatchDocument<Api.Apprentice>().Replace(x => x.Email, message.NewEmailAddress);
            return _api.UpdateApprentice(message.ApprenticeId, requestBody);
        }
    }
}
