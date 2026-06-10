using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Bookings;

public class BookingDto
{
    public int Id { get; init; }
    public int CenterId { get; init; }
    public string? CenterName { get; init; }
    public int BranchId { get; init; }
    public string? BranchName { get; init; }
    public int ServiceId { get; init; }
    public string? ServiceName { get; init; }
    public int? StaffId { get; init; }
    public string? StaffName { get; init; }
    public DateOnly BookingDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public BookingStatus Status { get; init; }
    public string? CustomerNotes { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTimeOffset Created { get; init; }

    public static BookingDto FromEntity(Booking b) => new()
    {
        Id = b.Id,
        CenterId = b.CenterId,
        CenterName = b.Center?.Name,
        BranchId = b.BranchId,
        BranchName = b.Branch?.Name,
        ServiceId = b.ServiceId,
        ServiceName = b.Service?.Name,
        StaffId = b.StaffId,
        StaffName = b.Staff?.Name,
        BookingDate = b.BookingDate,
        StartTime = b.StartTime,
        EndTime = b.EndTime,
        Status = b.Status,
        CustomerNotes = b.CustomerNotes,
        TotalAmount = b.TotalAmount,
        Created = b.Created
    };
}
