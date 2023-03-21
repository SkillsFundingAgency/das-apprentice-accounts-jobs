namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure.OuterApi
{
    public class EmployerConfirmationRequest
    {
        public EmployerConfirmationRequest(bool employerCorrect)
        {
            EmployerCorrect = employerCorrect;
        }

        public bool EmployerCorrect { get; }
    }
}