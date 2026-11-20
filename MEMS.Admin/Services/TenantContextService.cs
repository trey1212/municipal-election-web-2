using MEMS.Data.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MEMS.Admin.Services;

public class TenantContextService(
    AuthenticationStateProvider authProvider,
    UserManager<MunicipalUser> userManager)
{
    public async Task<(bool IsSuperAdmin, int? MunicipalityId)> GetCurrentTenantAsync()
    {
        var state = await authProvider.GetAuthenticationStateAsync();
        var user = state.User;

        if (user.IsInRole("SuperAdmin"))
            return (true, null);

        var claim = user.FindFirst("MunicipalityId")?.Value;
        if (int.TryParse(claim, out var id))
            return (false, id);

        var appUser = await userManager.GetUserAsync(user);
        return (false, appUser?.MunicipalityId);
    }
}