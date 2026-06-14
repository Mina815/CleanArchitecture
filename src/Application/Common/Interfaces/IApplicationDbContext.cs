using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BeautyCenter> BeautyCenters { get; }
    DbSet<Branch> Branches { get; }
    DbSet<Domain.Entities.Service> Services { get; }
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<Staff> StaffMembers { get; }
    DbSet<WorkingHour> WorkingHours { get; }
    DbSet<TimeOff> TimeOffs { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<CenterImage> CenterImages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
