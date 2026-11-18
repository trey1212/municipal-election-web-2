using System.ComponentModel.DataAnnotations;

namespace MEMS.Shared.DTOs;

public class MunicipalityDto
{
    public int MunicipalityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public DateTime ElectionDate { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
}

public class CreateMunicipalityDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug must be lowercase letters, numbers, or hyphens.")]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Province { get; set; } = string.Empty;

    [Required]
    public DateTime ElectionDate { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public string? LogoUrl { get; set; }
}