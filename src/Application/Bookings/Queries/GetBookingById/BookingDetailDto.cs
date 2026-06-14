namespace CleanArchitecture.Application.Bookings.Queries.GetBookingById;

public class BookingDetailDto
{
    public int Id { get; init; }
    public string CustomerId { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public int CenterId { get; init; }
    public string CenterName { get; init; } = string.Empty;
    public int BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public int ServiceId { get; init; }
    public string ServiceName { get; init; } = string.Empty;
    public int? StaffId { get; init; }
    public string? StaffName { get; init; }
    public DateOnly BookingDate { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? CustomerNotes { get; init; }
    public string? CancellationReason { get; init; }
    public DateTimeOffset? ConfirmedAt { get; init; }
    public DateTimeOffset? CancelledAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public decimal ServicePrice { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTimeOffset Created { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Booking, BookingDetailDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.CustomerName, opt => opt.Ignore())
                .ForMember(d => d.CenterName, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.Ignore())
                .ForMember(d => d.ServiceName, opt => opt.Ignore())
                .ForMember(d => d.StaffName, opt => opt.Ignore());
        }
    }
}
