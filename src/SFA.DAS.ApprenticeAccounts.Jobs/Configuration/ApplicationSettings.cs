using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Configuration
{
    public class ApplicationSettings
    {
        public ApiOptions ApprenticePortalOuterApi { get; set; } = null!;
    }

    public class ApiOptions : IApimClientConfiguration
    {
        public const string ApprenticePortalOuterApi = "ApprenticePortalOuterApi";
        public string ApiBaseUrl { get; set; } = null!;
        public string IdentifierUri { get; set; } = null!;
        public string SubscriptionKey { get; set; } = null!;
        public string ApiVersion { get; set; } = null!;
    }
}
