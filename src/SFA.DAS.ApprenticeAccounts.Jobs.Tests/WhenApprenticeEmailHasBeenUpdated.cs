using AutoFixture.NUnit3;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.LoginServiceEventHandlers;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Tests
{
    public class WhenApprenticeEmailHasBeenUpdated
    {
        [Test, AutoMoqData]
        public async Task Then_notify_apim(
            [Frozen] Mock<IOuterApiClient> api,
            [Frozen] Mock<ILogger<UpdateEmailAddressCommandHandler>> logger,
            UpdateEmailAddressCommandHandler sut,
            UpdateEmailAddressCommand evt
            )
        {
            await sut.Handle(evt, new TestableMessageHandlerContext());

            using (new AssertionScope())
            {
                logger.Verify((x => x.Log(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((object v, Type _) =>
                           v.ToString().Contains($"Received UpdateEmailAddressCommand for apprentice")),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>())));
                api.Verify(m => m.UpdateApprentice(
                    evt.ApprenticeId,
                    It.Is<JsonPatchDocument<Api.Apprentice>>(x => ReplacesEmailAddress(evt, x))));
            }
           
           
        }

        private bool ReplacesEmailAddress(UpdateEmailAddressCommand evt, JsonPatchDocument<Api.Apprentice> n)
        {
            return n.Operations.Count == 1
                && n.Operations[0].OperationType == OperationType.Replace
                && n.Operations[0].path == "/Email"
                && (string)n.Operations[0].value == evt.NewEmailAddress;
        }
    }
}