namespace CleanArchitecture.Application.Bookings.Queries.GetMyBookings;

public class BookingDto
{
    public int Id { get; init; }
    public int CenterId { get; init; }
    public string CenterName { get; init; } = string.Empty;
    public int BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public string ServiceName { get; init; } = string.Empty;
    public DateOnly BookingDate { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.CenterName, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.Ignore())
                .ForMember(d => d.ServiceName, opt => opt.Ignore());
        }
    }
}
