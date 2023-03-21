using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class ApprenticeshipAssociation
    {
        public string RegistrationId { get; set; } = null!;
        public Guid ApprenticeId { get; set; }
    }
}