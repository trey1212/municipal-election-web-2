using System.Text.Json;
using Microsoft.JSInterop;

namespace MEMS.Public.Services;

public class VotingGuideSessionService(IJSRuntime js)
{
    private static string StorageKey(string tenantSlug) =>
        $"mems_ballot_{tenantSlug.Trim().ToLowerInvariant()}";

    public async Task<List<int>> GetSelectedCandidateIdsAsync(string tenantSlug)
    {
        var json = await js.InvokeAsync<string?>("sessionStorage.getItem", StorageKey(tenantSlug));
        return string.IsNullOrWhiteSpace(json)
            ? []
            : JsonSerializer.Deserialize<List<int>>(json) ?? [];
    }

    public async Task ToggleCandidateAsync(string tenantSlug, int candidateId)
    {
        var ids = await GetSelectedCandidateIdsAsync(tenantSlug);
        if (!ids.Remove(candidateId))
            ids.Add(candidateId);

        await SaveAsync(tenantSlug, ids);
    }

    public async Task RemoveCandidateAsync(string tenantSlug, int candidateId)
    {
        var ids = await GetSelectedCandidateIdsAsync(tenantSlug);
        if (ids.Remove(candidateId))
            await SaveAsync(tenantSlug, ids);
    }

    private async Task SaveAsync(string tenantSlug, List<int> ids)
    {
        var json = JsonSerializer.Serialize(ids);
        await js.InvokeVoidAsync("sessionStorage.setItem", StorageKey(tenantSlug), json);
    }
}