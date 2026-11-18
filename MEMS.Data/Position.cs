using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEMS.Data.Models;

public class Position
{
    public int PositionId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; } // e.g., Mayor, Councilor, School Trustee

    [Range(1, 100)]
    public int SeatsAvailable { get; set; }

    // Forgein key
    public int MunicipalityId { get; set; }
    public Municipality? Municipality { get; set; }

    public List<Candidate> Candidates { get; set; } = new List<Candidate>();
}