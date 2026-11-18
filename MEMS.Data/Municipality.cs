using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEMS.Data.Models;

public class Municipality
{
    public int MunicipalityId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug must be lowercase letters, numbers, or hyphens.")]
    public string? Slug { get; set; } // e.g., "burnaby", "surrey", "richmond"

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string? Province { get; set; }

    public DateTime ElectionDate { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public string? LogoUrl { get; set; }

    // Navigation properties
    public List<Position> Positions { get; set; } = new();
    public List<Candidate> Candidates { get; set; } = new();
}