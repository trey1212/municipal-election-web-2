using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEMS.Data;
using MEMS.Data.Models;

namespace MEMS.Api.Controllers;

// Base controller for the tenants to avoid repeating code
[ApiController]
public abstract class TenantControllerBase(ApplicationDbContext db) : ControllerBase
{
    protected readonly ApplicationDbContext _db = db;

    protected async Task<Municipality?> ResolveTenantAsync(string tenantSlug)
    {
        return await _db.Municipalities
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Slug == tenantSlug.ToLower());
    }

    protected bool CanManageTenant(int municipalityId)
    {
        if (User.IsInRole("SuperAdmin")) return true;

        var claim = User.FindFirst("MunicipalityId")?.Value;
        return int.TryParse(claim, out var userMuniId) && userMuniId == municipalityId;
    }
}