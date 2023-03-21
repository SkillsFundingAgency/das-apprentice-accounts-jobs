namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public static class ApprenticeshipExtensions
    {
        public static bool IsCompleted(this Apprenticeship apprenticeship) =>
            apprenticeship.ConfirmedOn != null;
    }
}