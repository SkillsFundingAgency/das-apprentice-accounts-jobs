using System;

#nullable disable

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class VerifyRegistrationResponse
    {
        public Guid ApprenticeId { get; set; }
        public string Email { get; set; }
        public bool HasViewedVerification { get; set; }
        public bool HasCompletedVerification { get; set; }
    }
}