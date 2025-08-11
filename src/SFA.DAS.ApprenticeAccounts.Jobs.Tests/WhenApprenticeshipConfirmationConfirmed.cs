using AutoFixture.NUnit3;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.ApprenticeshipConfirmationEventHandlers;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Tests
{
    public class WhenApprenticeshipConfirmationConfirmed
    {
        [Test, AutoMoqData]
        public async Task Then_notify_apim(
            [Frozen] Mock<IOuterApiClient> api,
            [Frozen] Mock<ILogger<ApprenticeshipConfirmationConfirmedHandler>> logger,
            ApprenticeshipConfirmationConfirmedHandler sut,
            ApprenticeshipConfirmationConfirmedEvent evt
            )
        {
            ApprenticeshipConfirmedRequest expected = new ApprenticeshipConfirmedRequest
            {
                CommitmentsApprenticeshipId = evt.CommitmentsApprenticeshipId,
                ApprovedOn = evt.CommitmentsApprovedOn
            };

            await sut.Handle(evt, new TestableMessageHandlerContext());

            using (new AssertionScope())
            {
                logger.Verify((x => x.Log(LogLevel.Information,
                       It.IsAny<EventId>(),
                       It.Is<It.IsAnyType>((object v, Type _) =>
                       v.ToString().Contains($"Received ApprenticeshipConfirmationConfirmedEvent for apprentice")),
                       It.IsAny<Exception>(),
                       (Func<It.IsAnyType, Exception, string>)It.IsAny<object>())));
                api.Verify(m => m.SendApprenticeshipConfirmed(
                evt.ApprenticeId,
                It.Is<ApprenticeshipConfirmedRequest>(e =>
                    e.ApprovedOn == expected.ApprovedOn
                    && e.CommitmentsApprenticeshipId == expected.CommitmentsApprenticeshipId
            )));
            }
        }
    }
}