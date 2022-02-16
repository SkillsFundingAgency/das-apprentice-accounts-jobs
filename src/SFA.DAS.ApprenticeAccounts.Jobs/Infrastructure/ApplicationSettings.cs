using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    public class ApplicationSettings
    {
        public ApiOptions ApprenticeAccountsApi { get; set; } = null!;
    }

    public class ApiOptions : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; } = null!;
        public string Identifier { get; set; } = null!;
    }
}