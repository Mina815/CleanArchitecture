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

        await app.MigrateDatabaseAsync();

        await app.SeedDatabaseAsync();

    }

    public static async Task MigrateDatabaseAsync(this WebApplication app)

    {

        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.MigrateAsync();

    }



    public static async Task SeedDatabaseAsync(this WebApplication app)

    {

        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.SeedAsync();

    }



   

}



public class ApplicationDbContextInitialiser

{

    private readonly ILogger<ApplicationDbContextInitialiser> _logger;

    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;



    public ApplicationDbContextInitialiser(

        ILogger<ApplicationDbContextInitialiser> logger,

        ApplicationDbContext context,

        UserManager<ApplicationUser> userManager,

        RoleManager<IdentityRole> roleManager)

    {

        _logger = logger;

        _context = context;

        _userManager = userManager;

        _roleManager = roleManager;

    }



    public async Task MigrateAsync()

    {

        try

        {

            await _context.Database.MigrateAsync();

        }

        catch (Exception ex)

        {

            _logger.LogError(ex, "An error occurred while migrating the database.");

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

        foreach (var role in new[] { Roles.Administrator, Roles.Customer, Roles.Provider })

        {

            if (_roleManager.Roles.All(r => r.Name != role))

                await _roleManager.CreateAsync(new IdentityRole(role));

        }



        var admin = await CreateUserAsync("01000000000", "Admin User", "Admin123!", UserRole.Admin, Roles.Administrator);

        var provider = await CreateUserAsync("01011111111", "Sara Beauty Owner", "Provider123!", UserRole.Provider, Roles.Provider);

        var customer = await CreateUserAsync("01022222222", "Ahmed Customer", "Customer123!", UserRole.Customer, Roles.Customer);



        if (!_context.ServiceCategories.Any())

        {

            var categories = new[]

            {

                new ServiceCategory { Name = "Hair", NameAr = "شعر", DisplayOrder = 1, IsActive = true },

                new ServiceCategory { Name = "Nails", NameAr = "أظافر", DisplayOrder = 2, IsActive = true },

                new ServiceCategory { Name = "Skin Care", NameAr = "عناية بالبشرة", DisplayOrder = 3, IsActive = true },

                new ServiceCategory { Name = "Makeup", NameAr = "مكياج", DisplayOrder = 4, IsActive = true }

            };

            _context.ServiceCategories.AddRange(categories);

            await _context.SaveChangesAsync();



            var center = new BeautyCenter

            {

                OwnerId = provider!.Id,

                Name = "Jamalek Beauty Lounge",

                NameAr = "صالون جمالك",

                Description = "Premium beauty center in Cairo with expert stylists.",

                DescriptionAr = "مركز تجميل راقي في القاهرة مع خبراء تجميل.",

                LogoUrl = "/images/jamalek-logo.png",

                IsActive = true,

                IsVerified = true,

                AverageRating = 4.5m,

                TotalReviews = 2

            };

            _context.BeautyCenters.Add(center);

            await _context.SaveChangesAsync();



            var branchMaadi = new Branch

            {

                CenterId = center.Id,

                Name = "Maadi Branch",

                NameAr = "فرع المعادي",

                Address = "Road 9, Maadi",

                City = "Cairo",

                District = "Maadi",

                Latitude = 29.9602,

                Longitude = 31.2569,

                Phone = "0227501234",

                WhatsappNumber = "01011111111",

                IsActive = true

            };

            var branchZamalek = new Branch

            {

                CenterId = center.Id,

                Name = "Zamalek Branch",

                NameAr = "فرع الزمالك",

                Address = "26th July St, Zamalek",

                City = "Cairo",

                District = "Zamalek",

                Latitude = 30.0626,

                Longitude = 31.2197,

                Phone = "0227355678",

                IsActive = true

            };

            _context.Branches.AddRange(branchMaadi, branchZamalek);

            await _context.SaveChangesAsync();



            foreach (var branch in new[] { branchMaadi, branchZamalek })

            {

                for (var day = DayOfWeek.Sunday; day <= DayOfWeek.Saturday; day++)

                {

                    _context.WorkingHours.Add(new WorkingHour

                    {

                        BranchId = branch.Id,

                        DayOfWeek = day,

                        OpenTime = new TimeOnly(10, 0),

                        CloseTime = new TimeOnly(22, 0),

                        IsClosed = day == DayOfWeek.Friday

                    });

                }

            }



            var hairCat = categories[0];

            var services = new[]

            {

                new CenterService { CenterId = center.Id, CategoryId = hairCat.Id, Name = "Haircut", NameAr = "قص شعر", Price = 150, DurationMinutes = 30, DisplayOrder = 1, IsActive = true },

                new CenterService { CenterId = center.Id, CategoryId = hairCat.Id, Name = "Hair Color", NameAr = "صبغة شعر", Price = 500, DurationMinutes = 90, DisplayOrder = 2, IsActive = true },

                new CenterService { CenterId = center.Id, CategoryId = categories[1].Id, Name = "Manicure", NameAr = "مانيكير", Price = 120, DurationMinutes = 45, DisplayOrder = 3, IsActive = true },

                new CenterService { CenterId = center.Id, CategoryId = categories[3].Id, Name = "Bridal Makeup", NameAr = "مكياج عروس", Price = 2000, DurationMinutes = 120, DisplayOrder = 4, IsActive = true }

            };

            _context.CenterServices.AddRange(services);



            _context.CenterImages.Add(new CenterImage

            {

                CenterId = center.Id,

                ImageUrl = "/images/center-1.jpg",

                Caption = "Main salon",

                DisplayOrder = 1,

                IsPrimary = true

            });



            var staff1 = new Staff { BranchId = branchMaadi.Id, Name = "Nour Hassan", Specialization = "Hair Stylist", IsActive = true };

            var staff2 = new Staff { BranchId = branchMaadi.Id, Name = "Mona Ali", Specialization = "Makeup Artist", IsActive = true };

            _context.StaffMembers.AddRange(staff1, staff2);

            await _context.SaveChangesAsync();



            var completedBooking = Booking.Create(

                customer!.Id, center.Id, branchMaadi.Id, services[0].Id, staff1.Id,

                DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),

                new TimeOnly(14, 0), new TimeOnly(14, 30),

                services[0].Price, "Please use organic products");

            completedBooking.Confirm();

            completedBooking.Complete();

            _context.Bookings.Add(completedBooking);

            await _context.SaveChangesAsync();



            _context.Reviews.Add(Review.Create(customer.Id, center.Id, completedBooking.Id, 5, "Excellent service!"));

            await _context.SaveChangesAsync();

        }

    }



    private async Task<ApplicationUser?> CreateUserAsync(string phone, string name, string password, UserRole role, string roleName)

    {

        if (_userManager.Users.Any(u => u.PhoneNumber == phone))

            return await _userManager.Users.FirstAsync(u => u.PhoneNumber == phone);



        var user = new ApplicationUser

        {

            UserName = phone,

            PhoneNumber = phone,

            Email = $"{phone}@jamalek.local",

            Name = name,

            Role = role,

            IsActive = true

        };



        await _userManager.CreateAsync(user, password);

        await _userManager.AddToRoleAsync(user, roleName);

        return user;

    }

}


