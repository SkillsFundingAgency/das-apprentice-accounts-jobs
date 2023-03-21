namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure.OuterApi
{
    public static class ApprenticeshipExtensions
    {
        public static bool IsCompleted(this Apprenticeship apprenticeship) =>
            apprenticeship.ConfirmedOn != null;
    }
}