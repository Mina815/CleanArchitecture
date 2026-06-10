using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<BeautyCenter> BeautyCenters { get; }
    DbSet<Branch> Branches { get; }
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<CenterService> CenterServices { get; }
    DbSet<Domain.Entities.Staff> StaffMembers { get; }
    DbSet<WorkingHour> WorkingHours { get; }
    DbSet<TimeOff> TimeOffs { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<CenterImage> CenterImages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
