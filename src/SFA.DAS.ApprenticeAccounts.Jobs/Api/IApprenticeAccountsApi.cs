using Microsoft.AspNetCore.JsonPatch;
using RestEase;
using System.Threading.Tasks;
using System;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    internal interface IApprenticeAccountsApi
    {
        [Patch("/apprentices/{apprenticeId}")]
        Task UpdateApprentice([Path] Guid apprenticeId, [Body] JsonPatchDocument<Apprentice> patch);
    }
}
