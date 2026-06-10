using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Bookings.Queries.GetAvailability;

public record GetAvailabilityQuery(int BranchId, int ServiceId, DateOnly Date, int? StaffId) : IRequest<List<TimeSlotDto>>;

public class GetAvailabilityQueryHandler : IRequestHandler<GetAvailabilityQuery, List<TimeSlotDto>>
{
    private readonly IAvailabilityService _availabilityService;

    public GetAvailabilityQueryHandler(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    public Task<List<TimeSlotDto>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
        => _availabilityService.GetAvailableSlotsAsync(request.BranchId, request.ServiceId, request.Date, request.StaffId, cancellationToken);
}
