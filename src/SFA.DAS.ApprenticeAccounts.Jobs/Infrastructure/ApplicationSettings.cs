using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    public class ApplicationSettings
    {
        public ApiOptions ApprenticeAccountsApi { get; set; } = null!;
    }

    public class ApiOptions : IApimClientConfiguration
    {
        public const string ApprenticeAccountsInternalApi = "ApprenticeAccountsInternalApi";
        public string ApiBaseUrl { get; set; } = null!;
        public string IdentifierUri { get; set; } = null!;
        public string SubscriptionKey { get; set; } = null!;
        public string ApiVersion { get; set; } = null!;
    }
  

    public class OuterApiConfiguration : IApimClientConfiguration
    {
        public string ApiBaseUrl { get; set; } = null!;
        public string SubscriptionKey { get; set; } = null!;
        public string ApiVersion { get; set; } = null!;
    }

}