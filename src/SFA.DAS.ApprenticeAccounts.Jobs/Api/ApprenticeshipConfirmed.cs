using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class ApprenticeshipConfirmed
    {
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}