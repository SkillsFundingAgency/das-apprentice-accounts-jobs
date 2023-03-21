namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
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