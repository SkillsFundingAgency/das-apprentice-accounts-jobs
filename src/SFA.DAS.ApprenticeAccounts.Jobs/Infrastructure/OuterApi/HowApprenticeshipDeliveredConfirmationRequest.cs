namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure.OuterApi
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