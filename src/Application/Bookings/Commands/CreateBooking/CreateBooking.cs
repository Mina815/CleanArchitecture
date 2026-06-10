using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.Commands.CreateBooking;

[Authorize(Roles = Roles.Customer)]
public record CreateBookingCommand : IRequest<int>
{
    public int BranchId { get; init; }
    public int ServiceId { get; init; }
    public int? StaffId { get; init; }
    public DateOnly BookingDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public string? CustomerNotes { get; init; }
    public PaymentProvider PaymentProvider { get; init; } = PaymentProvider.Cash;
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingValidationService _validationService;
    private readonly IUser _user;
    private readonly IPaymentServiceFactory _paymentFactory;

    public CreateBookingCommandHandler(
        IApplicationDbContext context,
        IBookingRepository bookingRepository,
        IBookingValidationService validationService,
        IUser user,
        IPaymentServiceFactory paymentFactory)
    {
        _context = context;
        _bookingRepository = bookingRepository;
        _validationService = validationService;
        _user = user;
        _paymentFactory = paymentFactory;
    }

    public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        await _validationService.ValidateBookingAsync(
            request.BranchId, request.ServiceId, request.BookingDate, request.StartTime, request.StaffId, cancellationToken: cancellationToken);

        var service = await _context.CenterServices.AsNoTracking()
            .FirstAsync(s => s.Id == request.ServiceId, cancellationToken);
        var branch = await _context.Branches.AsNoTracking()
            .FirstAsync(b => b.Id == request.BranchId, cancellationToken);

        var endTime = request.StartTime.AddMinutes(service.DurationMinutes);
        var booking = Booking.Create(
            _user.Id!, branch.CenterId, request.BranchId, request.ServiceId,
            request.StaffId, request.BookingDate, request.StartTime, endTime,
            service.Price, request.CustomerNotes);

        _bookingRepository.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        booking.RaiseCreatedEvent();

        var paymentService = _paymentFactory.GetService(request.PaymentProvider);
        var paymentResult = await paymentService.InitiatePaymentAsync(booking.Id, service.Price, "EGP", cancellationToken);

        var payment = new Payment
        {
            BookingId = booking.Id,
            Provider = request.PaymentProvider,
            Amount = service.Price,
            Status = request.PaymentProvider == PaymentProvider.Cash ? PaymentStatus.Pending : PaymentStatus.Pending,
            PaymentUrl = paymentResult.PaymentUrl,
            ProviderReference = paymentResult.ProviderReference,
            TransactionId = paymentResult.TransactionId
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}
