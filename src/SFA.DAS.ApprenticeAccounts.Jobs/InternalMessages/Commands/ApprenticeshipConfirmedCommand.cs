using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.InternalMessages.Commands
{
    public class ApprenticeshipConfirmedCommand
    {
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}