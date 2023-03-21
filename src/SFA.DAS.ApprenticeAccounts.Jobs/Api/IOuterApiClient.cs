using Microsoft.AspNetCore.JsonPatch;
using RestEase;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public interface IOuterApiClient
    {
        [Get("/apprentices/{id}")]
        Task<Apprentice> GetApprentice([Path] Guid id);

        [Patch("/apprentices/{apprenticeId}")]
        Task UpdateApprentice([Path] Guid apprenticeId, [Body] JsonPatchDocument<Apprentice> patch);
    }
}