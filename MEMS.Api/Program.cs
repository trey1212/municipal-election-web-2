using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MEMS.Data;
using MEMS.Data.Models;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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