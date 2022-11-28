using RestEase;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public interface IApprenticeAccountsApi
    {
        [Get("/apprentices/{apprenticeid}")]
        Task<Apprentice> GetApprentice([Path] Guid apprenticeid);

        [Patch("/apprentices/{apprenticeId}")]
        Task UpdateApprentice([Path] Guid apprenticeId, [Body] JsonPatchDocument<Apprentice> patch);
    }
}