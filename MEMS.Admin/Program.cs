using MEMS.Admin.Components;
using MEMS.Admin.Services;
using MEMS.Data;
using MEMS.Data.Models;
using Microsoft.AspNetCore.Identity;
using MEMS.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<ApplicationDbContext>("memsdb");

builder.Services.AddIdentity<MunicipalUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<TenantContextService>();
builder.Services.AddScoped<ImageUploadService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/auth/login", async (
    HttpContext http,
    SignInManager<MunicipalUser> signInManager) =>
{
    var form = await http.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    var result = await signInManager.PasswordSignInAsync(
        username, password, isPersistent: false, lockoutOnFailure: false);

    return result.Succeeded
        ? Results.Redirect("/admin")
        : Results.Redirect("/login?error=1");
});

app.MapPost("/auth/logout", async (SignInManager<MunicipalUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
