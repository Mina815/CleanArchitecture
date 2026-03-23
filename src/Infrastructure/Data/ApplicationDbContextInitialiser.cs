using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            //await _context.Database.EnsureDeletedAsync();
            //await _context.Database.EnsureCreatedAsync();

            // NEW 
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
        foreach (var roleName in new[] { Roles.Administrator, Roles.Customer, Roles.Provider })
        {
            if (_roleManager.Roles.All(r => r.Name != roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost", FullName = "Administrator" };
        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            await _userManager.AddToRolesAsync(administrator, [Roles.Administrator]);
        }

        var provider = new ApplicationUser { UserName = "provider@localhost", Email = "provider@localhost", FullName = "Demo Provider" };
        if (_userManager.Users.All(u => u.UserName != provider.UserName))
        {
            await _userManager.CreateAsync(provider, "Provider123!");
            await _userManager.AddToRolesAsync(provider, [Roles.Provider]);
        }

        var customer = new ApplicationUser { UserName = "customer@localhost", Email = "customer@localhost", FullName = "Demo Customer" };
        if (_userManager.Users.All(u => u.UserName != customer.UserName))
        {
            await _userManager.CreateAsync(customer, "Customer123!");
            await _userManager.AddToRolesAsync(customer, [Roles.Customer]);
        }

        if (await _context.ServiceCategories.AnyAsync())
            return;

        var category = new ServiceCategory
        {
            Name = "Hair & Styling",
            NameAr = "تصفيف الشعر",
            DisplayOrder = 1,
            IsActive = true
        };
        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        var providerUser = await _userManager.FindByEmailAsync("provider@localhost");
        Guard.Against.Null(providerUser);

        var center = new BeautyCenter
        {
            OwnerId = providerUser.Id,
            Name = "Jamalek Demo Salon",
            NameAr = "صالون جمالك التجريبي",
            Description = "Demo beauty center for Jamalek.",
            DescriptionAr = "مركز تجميل تجريبي.",
            IsActive = true,
            IsVerified = true
        };
        _context.BeautyCenters.Add(center);
        await _context.SaveChangesAsync();

        var branch = new Branch
        {
            CenterId = center.Id,
            Name = "Zamalek Branch",
            NameAr = "فرع الزمالك",
            Address = "26th of July Street",
            City = "Cairo",
            District = "Zamalek",
            Phone = "+201234567890",
            IsActive = true
        };
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        for (var d = 0; d <= 6; d++)
        {
            _context.WorkingHours.Add(new WorkingHour
            {
                BranchId = branch.Id,
                DayOfWeek = d,
                IsClosed = d == 0,
                OpenTime = new TimeOnly(10, 0),
                CloseTime = new TimeOnly(22, 0)
            });
        }

        _context.Services.Add(new Service
        {
            CenterId = center.Id,
            CategoryId = category.Id,
            Name = "Women's haircut",
            NameAr = "قص شعر سيدات",
            Price = 250m,
            DurationMinutes = 60,
            DisplayOrder = 1,
            IsActive = true
        });

        await _context.SaveChangesAsync();
    }
}
