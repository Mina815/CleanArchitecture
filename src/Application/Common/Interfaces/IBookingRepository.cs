using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByBranchAndDateAsync(int branchId, DateOnly date, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetCustomerBookingsAsync(string customerId, CancellationToken cancellationToken = default);
    Task<bool> HasConflictAsync(int branchId, DateOnly date, TimeOnly startTime, TimeOnly endTime, int? staffId, int? excludeBookingId = null, CancellationToken cancellationToken = default);
    void Add(Booking booking);
}
