using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace CleanArchitecture.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogInformation("Migrations history table not found; creating it and retrying.");
            await _context.Database.ExecuteSqlRawAsync(
                "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\"MigrationId\" text NOT NULL, \"ProductVersion\" text NOT NULL);");
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var roles = new[] { Roles.Administrator, Roles.Customer, Roles.Provider };
        foreach (var role in roles)
        {
            if (_roleManager.Roles.All(r => r.Name != role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Default admin user
        var adminUser = new ApplicationUser { UserName = "admin@jamalek.com", Email = "admin@jamalek.com", FullName = "Admin", FullNameAr = "مدير" };
        if (_userManager.Users.All(u => u.UserName != adminUser.UserName))
        {
            await _userManager.CreateAsync(adminUser, "Admin123!");
            await _userManager.AddToRoleAsync(adminUser, Roles.Administrator);
        }

        // Default provider user
        var providerUser = new ApplicationUser { UserName = "provider@jamalek.com", Email = "provider@jamalek.com", FullName = "Beauty Center", FullNameAr = "مركز تجميل" };
        if (_userManager.Users.All(u => u.UserName != providerUser.UserName))
        {
            await _userManager.CreateAsync(providerUser, "Provider123!");
            await _userManager.AddToRoleAsync(providerUser, Roles.Provider);
        }

        // Default customer user
        var customerUser = new ApplicationUser { UserName = "customer@jamalek.com", Email = "customer@jamalek.com", FullName = "Customer", FullNameAr = "عميل" };
        if (_userManager.Users.All(u => u.UserName != customerUser.UserName))
        {
            await _userManager.CreateAsync(customerUser, "Customer123!");
            await _userManager.AddToRoleAsync(customerUser, Roles.Customer);
        }

        // Seed a sample beauty center
        if (!_context.BeautyCenters.Any())
        {
            var center = new BeautyCenter
            {
                OwnerId = (await _userManager.FindByNameAsync("provider@jamalek.com"))!.Id,
                Name = "Jamalek Beauty Center",
                NameAr = "مركز جمالك للتجميل",
                Description = "A premium beauty center offering a wide range of services.",
                DescriptionAr = "مركز تجميل راقي يقدم مجموعة واسعة من الخدمات.",
                IsActive = true,
                IsVerified = true
            };
            _context.BeautyCenters.Add(center);
            await _context.SaveChangesAsync();
        }
    }
}
