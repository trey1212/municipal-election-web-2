using System.ComponentModel.DataAnnotations;

namespace MEMS.Shared.DTOs;

public class CandidateDto
{
    public int CandidateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public int PositionId { get; set; }
    public string PositionName { get; set; } = string.Empty;
    public int MunicipalityId { get; set; }
}

public class CreateCandidateDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Biography { get; set; }

    public string? PhotoUrl { get; set; }

    [Url]
    public string? Website { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public int PositionId { get; set; }
}