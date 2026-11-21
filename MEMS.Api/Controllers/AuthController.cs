using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MEMS.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MEMS.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<MunicipalUser> userManager,
    SignInManager<MunicipalUser> signInManager,
    IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null)
        {
            return Unauthorized();
        }

        var result = await signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        var userClaims = await userManager.GetClaimsAsync(user);
        var name = userClaims.FirstOrDefault(claim => claim.Type == "fullName")?.Value ?? "";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(
                signingKey, SecurityAlgorithms.HmacSha256));

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            name,
            email = user.Email
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        var name = request.Name.Trim();
        var email = request.Email.Trim();

        if (name.Length < 2)
        {
            return BadRequest("Name must contain at least 2 characters.");
        }

        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return Conflict("An account with this email already exists.");
        }

        var user = new MunicipalUser
        {
            Email = email,
            UserName = email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(string.Join(" ", result.Errors.Select(error => error.Description)));
        }

        var nameResult = await userManager.AddClaimAsync(user, new Claim("fullName", name));

        if (!nameResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return StatusCode(500, "Could not save the account name.");
        }

        return Ok(new { message = "Account created successfully." });
    }
    [HttpPost("firebase-token")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public async Task<IActionResult> GetFirebaseToken(
    [FromServices] FirebaseAuth firebaseAuth)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrWhiteSpace(userId))
    {
        return Unauthorized();
    }

    var firebaseToken = await firebaseAuth.CreateCustomTokenAsync(userId);

    Response.Headers["Cache-Control"] = "no-store";

    return Ok(new { firebaseToken });
}
}

public record LoginDto(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

public record RegisterDto(
    [property: Required] string Name,
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

    