using Microsoft.AspNetCore.Identity;

namespace MEMS.Data.Models;

public class MunicipalUser : IdentityUser
{
    public int? MunicipalityId { get; set; }
    public Municipality? Municipality { get; set; }
}