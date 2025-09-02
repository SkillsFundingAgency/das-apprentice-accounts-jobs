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

        [Patch("/apprentices/{id}")]
        Task UpdateApprentice([Path] Guid id, [Body] JsonPatchDocument<Apprentice> patch);

        [Post("/apprentices/{id}/my-apprenticeship")]
        Task SendApprenticeshipConfirmed([Path] Guid id, [Body] ApprenticeshipConfirmedRequest message);
    }
}