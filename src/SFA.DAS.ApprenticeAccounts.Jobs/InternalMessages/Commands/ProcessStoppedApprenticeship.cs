using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.InternalMessages.Commands
{
    public class ProcessStoppedApprenticeship
    {
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime CommitmentsStoppedOn { get; set; }
    }
}