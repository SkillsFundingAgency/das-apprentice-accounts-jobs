﻿namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public class EmployerConfirmationRequest
    {
        public EmployerConfirmationRequest(bool employerCorrect)
        {
            EmployerCorrect = employerCorrect;
        }

        public bool EmployerCorrect { get; }
    }
}