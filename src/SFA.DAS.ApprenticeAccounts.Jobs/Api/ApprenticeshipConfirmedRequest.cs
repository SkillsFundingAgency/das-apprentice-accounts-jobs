namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class ApprenticeshipConfirmedRequest
    {
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}