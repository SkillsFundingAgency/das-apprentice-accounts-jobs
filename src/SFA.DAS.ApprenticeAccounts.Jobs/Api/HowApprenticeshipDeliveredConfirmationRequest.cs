namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class HowApprenticeshipDeliveredConfirmationRequest
    {
        public HowApprenticeshipDeliveredConfirmationRequest(bool howApprenticeshipDeliveredCorrect)
        {
            HowApprenticeshipDeliveredCorrect = howApprenticeshipDeliveredCorrect;
        }

        public bool HowApprenticeshipDeliveredCorrect { get; }
    }
}