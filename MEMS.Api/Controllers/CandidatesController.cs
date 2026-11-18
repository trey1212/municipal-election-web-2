using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEMS.Data;
using MEMS.Data.Models;
using MEMS.Shared.DTOs;

namespace MEMS.Api.Controllers;

[Route("api/{tenantSlug}/[controller]")]
public class CandidatesController(ApplicationDbContext db) : TenantControllerBase(db)
{
    [HttpGet]
    public async Task<ActionResult<List<CandidateDto>>> GetAll(
        string tenantSlug,
        [FromQuery] string? search = null,
        [FromQuery] int? positionId = null)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound($"Municipality '{tenantSlug}' not found.");

        var query = _db.Candidates
            .AsNoTracking()
            .Include(c => c.Position)
            .Where(c => c.MunicipalityId == tenant.MunicipalityId);

        if (positionId.HasValue)
            query = query.Where(c => c.PositionId == positionId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name != null && c.Name.Contains(search));

        var results = await query
            .Select(c => new CandidateDto
            {
                CandidateId = c.CandidateId,
                Name = c.Name ?? string.Empty,
                Biography = c.Biography,
                PhotoUrl = c.PhotoUrl,
                Website = c.Website,
                Email = c.Email,
                PositionId = c.PositionId,
                PositionName = c.Position != null ? c.Position.Name ?? string.Empty : string.Empty,
                MunicipalityId = c.MunicipalityId
            })
            .ToListAsync();

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CandidateDto>> GetById(string tenantSlug, int id)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound();

        var c = await _db.Candidates
            .AsNoTracking()
            .Include(x => x.Position)
            .FirstOrDefaultAsync(x => x.CandidateId == id && x.MunicipalityId == tenant.MunicipalityId);

        if (c is null) return NotFound();

        return Ok(new CandidateDto
        {
            CandidateId = c.CandidateId,
            Name = c.Name ?? string.Empty,
            Biography = c.Biography,
            PhotoUrl = c.PhotoUrl,
            Website = c.Website,
            Email = c.Email,
            PositionId = c.PositionId,
            PositionName = c.Position?.Name ?? string.Empty,
            MunicipalityId = c.MunicipalityId
        });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,MunicipalityAdmin")]
    public async Task<ActionResult<CandidateDto>> Create(string tenantSlug, CreateCandidateDto dto)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound();
        if (!CanManageTenant(tenant.MunicipalityId)) return Forbid();

        // Ensure the chosen Position belongs to this same municipality!
        var position = await _db.Positions
            .FirstOrDefaultAsync(p => p.PositionId == dto.PositionId && p.MunicipalityId == tenant.MunicipalityId);
        if (position is null)
            return BadRequest("Selected position does not belong to this municipality.");

        var entity = new Candidate
        {
            Name = dto.Name,
            Biography = dto.Biography,
            PhotoUrl = dto.PhotoUrl,
            Website = dto.Website,
            Email = dto.Email,
            PositionId = dto.PositionId,
            MunicipalityId = tenant.MunicipalityId
        };

        _db.Candidates.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { tenantSlug, id = entity.CandidateId }, new CandidateDto
        {
            CandidateId = entity.CandidateId,
            Name = entity.Name!,
            Biography = entity.Biography,
            PhotoUrl = entity.PhotoUrl,
            Website = entity.Website,
            Email = entity.Email,
            PositionId = entity.PositionId,
            PositionName = position.Name ?? string.Empty,
            MunicipalityId = entity.MunicipalityId
        });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,MunicipalityAdmin")]
    public async Task<IActionResult> Update(string tenantSlug, int id, CreateCandidateDto dto)
    {
        var tenant = await ResolveTenantAsync(tenantSlug);
        if (tenant is null) return NotFound();
        if (!CanManageTenant(tenant.MunicipalityId)) return Forbid();

        var entity = await _db.Candidates
            .FirstOrDefaultAsync(c => c.CandidateId == id && c.MunicipalityId == tenant.MunicipalityId);
        if (entity is null) return NotFound();

        var validPosition = await _db.Positions
            .AnyAsync(p => p.PositionId == dto.PositionId && p.MunicipalityId == tenant.MunicipalityId);
        if (!validPosition)
            return BadRequest("Selected position does not belong to this municipality.");

        entity.Name = dto.Name;
        entity.Biography = dto.Biography;
        entity.PhotoUrl = dto.PhotoUrl;
        entity.Website = dto.Website;
        entity.Email = dto.Email;
        entity.PositionId = dto.PositionId;

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

        var entity = await _db.Candidates
            .FirstOrDefaultAsync(c => c.CandidateId == id && c.MunicipalityId == tenant.MunicipalityId);
        if (entity is null) return NotFound();

        _db.Candidates.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}