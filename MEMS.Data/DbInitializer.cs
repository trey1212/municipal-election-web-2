using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using MEMS.Data.Models;

namespace MEMS.Data;

public static class DbInitializer
{
    public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<MunicipalUser>>();

        string[] roleNames = { "SuperAdmin", "MunicipalityAdmin" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Helper to create seeded users cleanly
        async Task EnsureUserAsync(string username, string role, int? municipalityId)
        {
            if (await userManager.FindByNameAsync(username) == null)
            {
                var user = new MunicipalUser
                {
                    UserName = username,
                    Email = $"{username.ToLower()}@mems.ca",
                    EmailConfirmed = true,
                    MunicipalityId = municipalityId
                };

                var result = await userManager.CreateAsync(user, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    if (municipalityId.HasValue)
                    {
                        await userManager.AddClaimAsync(user, new Claim("MunicipalityId", municipalityId.Value.ToString()));
                    }
                }
            }
        }

        // Required A2 Seeded Users
        await EnsureUserAsync("Super", "SuperAdmin", null);
        await EnsureUserAsync("Burnabyadmin", "MunicipalityAdmin", 1);
        await EnsureUserAsync("Surreyadmin", "MunicipalityAdmin", 2);
        await EnsureUserAsync("Richmondadmin", "MunicipalityAdmin", 3);
    }
}