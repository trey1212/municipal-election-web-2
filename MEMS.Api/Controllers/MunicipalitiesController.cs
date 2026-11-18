using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEMS.Data;
using MEMS.Data.Models;
using MEMS.Shared.DTOs;

namespace MEMS.Api.Controllers;

[Route("api/[controller]")]
public class MunicipalitiesController(ApplicationDbContext db) : TenantControllerBase(db)
{
    [HttpGet]
    public async Task<ActionResult<List<MunicipalityDto>>> GetAll()
    {
        var items = await _db.Municipalities
            .AsNoTracking()
            .Select(m => new MunicipalityDto
            {
                MunicipalityId = m.MunicipalityId,
                Name = m.Name ?? string.Empty,
                Slug = m.Slug ?? string.Empty,
                Province = m.Province ?? string.Empty,
                ElectionDate = m.ElectionDate,
                Description = m.Description,
                LogoUrl = m.LogoUrl
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<MunicipalityDto>> GetBySlug(string slug)
    {
        var m = await ResolveTenantAsync(slug);
        if (m is null) return NotFound($"Municipality '{slug}' not found.");

        return Ok(new MunicipalityDto
        {
            MunicipalityId = m.MunicipalityId,
            Name = m.Name ?? string.Empty,
            Slug = m.Slug ?? string.Empty,
            Province = m.Province ?? string.Empty,
            ElectionDate = m.ElectionDate,
            Description = m.Description,
            LogoUrl = m.LogoUrl
        });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<MunicipalityDto>> Create(CreateMunicipalityDto dto)
    {
        var normalizedSlug = dto.Slug.Trim().ToLower();
        if (await _db.Municipalities.AnyAsync(m => m.Slug == normalizedSlug))
            return Conflict($"Slug '{normalizedSlug}' is already in use.");

        var entity = new Municipality
        {
            Name = dto.Name,
            Slug = normalizedSlug,
            Province = dto.Province,
            ElectionDate = dto.ElectionDate,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl
        };

        _db.Municipalities.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBySlug), new { slug = entity.Slug }, new MunicipalityDto
        {
            MunicipalityId = entity.MunicipalityId,
            Name = entity.Name!,
            Slug = entity.Slug!,
            Province = entity.Province!,
            ElectionDate = entity.ElectionDate,
            Description = entity.Description,
            LogoUrl = entity.LogoUrl
        });
    }

    [HttpPut("{slug}")]
    [Authorize(Roles = "SuperAdmin,MunicipalityAdmin")]
    public async Task<IActionResult> Update(string slug, CreateMunicipalityDto dto)
    {
        var entity = await _db.Municipalities.FirstOrDefaultAsync(m => m.Slug == slug.ToLower());
        if (entity is null) return NotFound();
        if (!CanManageTenant(entity.MunicipalityId)) return Forbid();

        entity.Name = dto.Name;
        entity.Province = dto.Province;
        entity.ElectionDate = dto.ElectionDate;
        entity.Description = dto.Description;
        entity.LogoUrl = dto.LogoUrl;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{slug}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(string slug)
    {
        var entity = await _db.Municipalities.FirstOrDefaultAsync(m => m.Slug == slug.ToLower());
        if (entity is null) return NotFound();

        _db.Municipalities.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}