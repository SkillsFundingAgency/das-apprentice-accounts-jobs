namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
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