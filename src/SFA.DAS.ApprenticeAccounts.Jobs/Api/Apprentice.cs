using System;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class Apprentice
    {
        public Guid ApprenticeId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}