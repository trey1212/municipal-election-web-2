using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MEMS.Data;
using MEMS.Data.Models;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Missing Jwt:SecretKey.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Missing Jwt:Issuer.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Missing Jwt:Audience.");

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.AddServiceDefaults();

// Wire up SQL Server DbContext via Aspire connection string "memsdb"
builder.AddSqlServerDbContext<ApplicationDbContext>("memsdb");

// Register Identity services for user/role seeding & auth
builder.Services.AddIdentity<MunicipalUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Allow Blazor WASM (MEMS.Public) to call the API from the browser
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddSingleton<FirebaseAuth>(services =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var credentialJson = configuration["Firebase:ServiceAccountJson"]
        ?? throw new InvalidOperationException(
            "Missing Firebase:ServiceAccountJson.");

    var credential = CredentialFactory
        .FromJson<ServiceAccountCredential>(credentialJson)
        .ToGoogleCredential();

    var firebaseApp = FirebaseApp.Create(new AppOptions
    {
        Credential = credential
    });

    return FirebaseAuth.GetAuth(firebaseApp);
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(); // Go to /scalar/v1 to test API

if (app.Environment.IsDevelopment())
{
    // Auto-migrate and seed SQL Server when Aspire starts up
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await DbInitializer.SeedRolesAndUsersAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();