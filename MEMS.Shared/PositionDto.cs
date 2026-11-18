using System.ComponentModel.DataAnnotations;

namespace MEMS.Shared.DTOs;

public class PositionDto
{
    public int PositionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SeatsAvailable { get; set; }
    public int MunicipalityId { get; set; }
}

public class CreatePositionDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 100)]
    public int SeatsAvailable { get; set; }
}