using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.InternalMessages.Commands
{
    public class RemindApprenticeCommand
    {
        public Guid RegistrationId { get; set; }
    }
}
