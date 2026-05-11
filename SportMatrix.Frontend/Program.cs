using SportMatrix.Frontend.Services;
using SportMatrix.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Register Infrastructure: DbContext + ASP.NET Core Identity
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<DemoDataService>();
builder.Services.AddSingleton<DashboardViewModelFactory>();

builder.Services.AddHttpClient<AthleteApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
});

builder.Services.AddHttpClient<FitnessApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
});

builder.Services.AddHttpClient<AIApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AiApiBaseUrl"]!);
});

var app = builder.Build();

// ── Seed default admin user on startup ──────────────────────────────────────
await SeedIdentityAsync(app);

// Force InvariantCulture so model binding always uses '.' as decimal separator
// and parses dates in ISO 8601 format — without this, the OS locale (uk-UA, ru-RU)
// causes Weight/Height/DateOfBirth form fields to silently fail to bind.
var invariantCulture = CultureInfo.InvariantCulture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(invariantCulture),
    SupportedCultures     = new[] { invariantCulture },
    SupportedUICultures   = new[] { invariantCulture }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ── Helper: seed roles + admin account ──────────────────────────────────────
static async Task SeedIdentityAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var config      = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // Ensure roles exist
    foreach (var role in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Create default admin from config (appsettings / env vars)
    var adminEmail    = config["Identity:AdminEmail"]    ?? "admin@sportmatrix.com";
    var adminPassword = config["Identity:AdminPassword"] ?? "Admin123!";

    // Seed Admin
    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
    if (existingAdmin is null)
    {
        var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }
    else
    {
        existingAdmin.EmailConfirmed = true;
        existingAdmin.UserName = adminEmail;
        existingAdmin.NormalizedUserName = userManager.NormalizeName(adminEmail);
        existingAdmin.NormalizedEmail = userManager.NormalizeEmail(adminEmail);
        await userManager.UpdateAsync(existingAdmin);

        var token = await userManager.GeneratePasswordResetTokenAsync(existingAdmin);
        await userManager.ResetPasswordAsync(existingAdmin, token, adminPassword);

        if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
            await userManager.AddToRoleAsync(existingAdmin, "Admin");
    }

    // Seed Mock Athletes
    var mockUsers = new[]
    {
        "d.kovalenko@sport.ua", "o.shevchenko@cycling.ua", "a.melnyk@fit.ua",
        "v.kravchenko@swim.ua", "i.bondarenko@power.ua", "n.moroz@yoga.ua",
        "s.tkachenko@tri.ua", "o.lysenko@trail.ua", "m.kravchuk@hiit.ua"
    };

    foreach (var email in mockUsers)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is null)
        {
            // User doesn't exist yet — create fresh
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, "Sport123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "User");
        }
        else
        {
            // User exists — ensure correct password and confirmed status
            existing.EmailConfirmed = true;
            await userManager.UpdateAsync(existing);

            var token = await userManager.GeneratePasswordResetTokenAsync(existing);
            await userManager.ResetPasswordAsync(existing, token, "Sport123!");

            if (!await userManager.IsInRoleAsync(existing, "User"))
                await userManager.AddToRoleAsync(existing, "User");
        }
    }
}
