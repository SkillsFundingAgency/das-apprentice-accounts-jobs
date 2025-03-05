﻿using Microsoft.AspNetCore.JsonPatch;
using RestEase;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Api
{
    public interface IOuterApiClient
    {
        [Get("/apprentices/{id}")]
        Task<Apprentice> GetApprentice([Path] Guid id);

        [Patch("/apprentices/{id}")]
        Task UpdateApprentice([Path] Guid id, [Body] JsonPatchDocument<Apprentice> patch);

        [Post("/apprentices/{id}/myapprenticeship")]
        Task SendApprenticeshipConfirmed([Path] Guid id, [Body] ApprenticeshipConfirmedRequest message);
    }
}