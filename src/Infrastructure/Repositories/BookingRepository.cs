using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Service)
            .Include(b => b.Branch)
            .Include(b => b.Center)
            .Include(b => b.Staff)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<List<Booking>> GetByBranchAndDateAsync(int branchId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Service)
            .Include(b => b.Staff)
            .Where(b => b.BranchId == branchId && b.BookingDate == date && b.Status != BookingStatus.Cancelled)
            .OrderBy(b => b.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetCustomerBookingsAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Service)
            .Include(b => b.Branch)
            .Include(b => b.Center)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasConflictAsync(int branchId, DateOnly date, TimeOnly startTime, TimeOnly endTime, int? staffId, int? excludeBookingId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Bookings
            .Where(b => b.BranchId == branchId
                && b.BookingDate == date
                && b.Status != BookingStatus.Cancelled
                && b.StartTime < endTime
                && b.EndTime > startTime);

        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        if (staffId.HasValue)
            query = query.Where(b => b.StaffId == null || b.StaffId == staffId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public void Add(Booking booking) => _context.Bookings.Add(booking);
}
