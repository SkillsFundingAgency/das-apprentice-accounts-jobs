using AutoFixture.NUnit3;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.ApprenticeAccounts.Jobs.Api;
using SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.LoginServiceEventHandlers;
using SFA.DAS.ApprenticeAccounts.Jobs.InternalMessages.Commands;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.ApprenticeshipConfirmationEventHandlers;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Tests
{
    public class WhenApprenticeshipConfirmationConfirmed
    {
        [Test, AutoMoqData]
        public async Task Then_notify_apim(
            [Frozen] Mock<IOuterApiClient> api,
            ApprenticeshipConfirmationConfirmedHandler sut,
            ApprenticeshipConfirmationConfirmedEvent evt
            )
        {
            ApprenticeshipConfirmedCommand expected = new ApprenticeshipConfirmedCommand
            {
                CommitmentsApprenticeshipId = evt.CommitmentsApprenticeshipId,
                ApprovedOn = evt.CommitmentsApprovedOn
            };

            await sut.Handle(evt, new TestableMessageHandlerContext());

            api.Verify(m => m.SendApprenticeshipConfirmed(
                evt.ApprenticeId,
                It.Is<ApprenticeshipConfirmedCommand>(e =>
                                                    e.ApprovedOn == expected.ApprovedOn
                                                    && e.CommitmentsApprenticeshipId == expected.CommitmentsApprenticeshipId
            )));
        }
    }
}