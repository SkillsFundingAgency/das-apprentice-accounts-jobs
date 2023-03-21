namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure.OuterApi
{
    public class RolesAndResponsibilitiesConfirmationRequest
    {
        public RolesAndResponsibilitiesConfirmationRequest(bool rolesAndResponsibilitiesCorrect)
        {
            RolesAndResponsibilitiesCorrect = rolesAndResponsibilitiesCorrect;
        }

        public bool RolesAndResponsibilitiesCorrect { get; }
    }
}