namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class ApprenticeshipDetailsConfirmationRequest
    {
        public ApprenticeshipDetailsConfirmationRequest(bool apprenticeshipDetailsCorrect)
        {
            ApprenticeshipDetailsCorrect = apprenticeshipDetailsCorrect;
        }

        public bool ApprenticeshipDetailsCorrect { get; }
    }
}