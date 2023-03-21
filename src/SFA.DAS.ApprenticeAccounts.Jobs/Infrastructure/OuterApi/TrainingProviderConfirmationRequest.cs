namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure.OuterApi
{
    public class TrainingProviderConfirmationRequest
    {
        public TrainingProviderConfirmationRequest(bool trainingProviderCorrect)
        {
            TrainingProviderCorrect = trainingProviderCorrect;
        }

        public bool TrainingProviderCorrect { get; }
    }
}