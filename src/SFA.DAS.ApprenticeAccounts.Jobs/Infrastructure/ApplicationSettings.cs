using SFA.DAS.Http.Configuration;
using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    public class ApplicationSettings
    {
        public ApiOptions ApprenticeAccountsApi { get; set; } = null!;
        //public UrlConfiguration ApprenticeWeb { get; set; } = null!;
        //public NotificationConfiguration Notifications { get; set; } = null!;
        //public Uri SurveyLink { get; set; } = null!;
    }

    public class UrlConfiguration
    {
        public Uri BaseUrl { get; set; } = null!;
        public Uri StartPageUrl => BaseUrl;
        public Uri ConfirmApprenticeshipUrl
            => new Uri(AddSubdomain("confirm", BaseUrl), "Apprenticeships");

        private static Uri AddSubdomain(string subdomain, Uri uri)
        {
            var builder = new UriBuilder(uri);
            builder.Host = $"{subdomain}.{builder.Host}";
            return builder.Uri;
        }
    }

    //public class NotificationConfiguration
    //{
    //    public Dictionary<string, string> Templates { get; set; } = new Dictionary<string, string>();

    //    public Guid ApprenticeSignUp => GetTemplateId(nameof(ApprenticeSignUp));
    //    public Guid ApprenticeshipChanged => GetTemplateId(nameof(ApprenticeshipChanged));
    //    public Guid ApprenticeshipConfirmed => GetTemplateId(nameof(ApprenticeshipConfirmed));
    //    public Guid ApprenticeshipStopped => GetTemplateId(nameof(ApprenticeshipStopped));


    //    private Guid GetTemplateId(string templateName)
    //    {
    //        return Templates.TryGetValue(templateName, out var templateId)
    //            ? Guid.Parse(templateId)
    //            : throw new MissingEmailTemplateConfigurationException(templateName);
    //    }
    //}

    public class ApiOptions : IApimClientConfiguration
    {
        public const string ApprenticeCommitmentsApi = "ApprenticeCommitmentsApi";
        public string ApiBaseUrl { get; set; } = null!;
        public string SubscriptionKey { get; set; } = null!;
        public string ApiVersion { get; set; } = null!;
    }
}