using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEMS.Data;
using MEMS.Data.Models;
using MEMS.Shared.DTOs;

namespace MEMS.Api.Controllers;

[Route("api/{tenantSlug}/[controller]")]
public class PositionsController(ApplicationDbContext db) : TenantControllerBase(db)
{
    [HttpGet]
    public async Task<ActionResult<List<PositionDto>>> GetAll(string tenantSlug)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound($"Municipality '{tenantSlug}' not found.");

        var positions = await _db.Positions
            .AsNoTracking()
            .Where(p => p.MunicipalityId == tenant.MunicipalityId)
            .Select(p => new PositionDto
            {
                PositionId = p.PositionId,
                Name = p.Name ?? string.Empty,
                SeatsAvailable = p.SeatsAvailable,
                MunicipalityId = p.MunicipalityId
            })
            .ToListAsync();

        return Ok(positions);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PositionDto>> GetById(string tenantSlug, int id)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound();

        var p = await _db.Positions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PositionId == id && x.MunicipalityId == tenant.MunicipalityId);

        if (p is null) return NotFound();

        return Ok(new PositionDto
        {
            PositionId = p.PositionId,
            Name = p.Name ?? string.Empty,
            SeatsAvailable = p.SeatsAvailable,
            MunicipalityId = p.MunicipalityId
        });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,MunicipalityAdmin")]
    public async Task<ActionResult<PositionDto>> Create(string tenantSlug, CreatePositionDto dto)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound();
        if (!CanManageTenant(tenant.MunicipalityId)) return Forbid();

        var entity = new Position
        {
            Name = dto.Name,
            SeatsAvailable = dto.SeatsAvailable,
            MunicipalityId = tenant.MunicipalityId
        };

        _db.Positions.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { tenantSlug, id = entity.PositionId }, new PositionDto
        {
            PositionId = entity.PositionId,
            Name = entity.Name!,
            SeatsAvailable = entity.SeatsAvailable,
            MunicipalityId = entity.MunicipalityId
        });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,MunicipalityAdmin")]
    public async Task<IActionResult> Update(string tenantSlug, int id, CreatePositionDto dto)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound();
        if (!CanManageTenant(tenant.MunicipalityId)) return Forbid();

        var entity = await _db.Positions
            .FirstOrDefaultAsync(p => p.PositionId == id && p.MunicipalityId == tenant.MunicipalityId);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        entity.SeatsAvailable = dto.SeatsAvailable;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin,MunicipalityAdmin")]
    public async Task<IActionResult> Delete(string tenantSlug, int id)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound();
        if (!CanManageTenant(tenant.MunicipalityId)) return Forbid();

        var entity = await _db.Positions
            .FirstOrDefaultAsync(p => p.PositionId == id && p.MunicipalityId == tenant.MunicipalityId);
        if (entity is null) return NotFound();

        _db.Positions.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}