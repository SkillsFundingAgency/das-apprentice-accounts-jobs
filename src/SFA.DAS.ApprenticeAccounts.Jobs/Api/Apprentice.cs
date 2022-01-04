using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class Apprentice
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
