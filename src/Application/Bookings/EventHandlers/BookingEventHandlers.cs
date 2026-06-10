using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class SendBookingCreatedNotificationHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IFcmService _fcmService;
    private readonly IBookingHubService _hubService;
    private readonly IUserProfileService _userProfile;

    public SendBookingCreatedNotificationHandler(
        IApplicationDbContext context, IFcmService fcmService, IBookingHubService hubService, IUserProfileService userProfile)
    {
        _context = context;
        _fcmService = fcmService;
        _hubService = hubService;
        _userProfile = userProfile;
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        var ownerId = await _userProfile.GetOwnerIdByCenterIdAsync(notification.CenterId, cancellationToken);
        var bookingData = new { notification.BookingId, notification.BookingDate, notification.StartTime, notification.BranchId };

        if (ownerId is not null)
        {
            var fcmToken = await _userProfile.GetFcmTokenAsync(ownerId, cancellationToken);
            await _fcmService.SendAsync(fcmToken, "New Booking", "You have a new booking request.", cancellationToken: cancellationToken);
            _context.Notifications.Add(new Notification
            {
                UserId = ownerId,
                Type = "BookingCreated",
                Title = "New Booking",
                Message = $"New booking on {notification.BookingDate}",
                ActionUrl = $"/provider/bookings/{notification.BookingId}"
            });
        }

        _context.Notifications.Add(new Notification
        {
            UserId = notification.CustomerId,
            Type = "BookingCreated",
            Title = "Booking Submitted",
            Message = "Your booking request has been submitted.",
            ActionUrl = $"/bookings/{notification.BookingId}"
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _hubService.NotifyNewBookingAsync(notification.BranchId, bookingData, cancellationToken);
    }
}

public class SendBookingConfirmedNotificationHandler : INotificationHandler<BookingConfirmedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IFcmService _fcmService;
    private readonly IBookingHubService _hubService;
    private readonly IUserProfileService _userProfile;

    public SendBookingConfirmedNotificationHandler(
        IApplicationDbContext context, IFcmService fcmService, IBookingHubService hubService, IUserProfileService userProfile)
    {
        _context = context;
        _fcmService = fcmService;
        _hubService = hubService;
        _userProfile = userProfile;
    }

    public async Task Handle(BookingConfirmedEvent notification, CancellationToken cancellationToken)
    {
        var fcmToken = await _userProfile.GetFcmTokenAsync(notification.CustomerId, cancellationToken);
        await _fcmService.SendAsync(fcmToken, "Booking Confirmed", "Your booking has been confirmed!", cancellationToken: cancellationToken);

        _context.Notifications.Add(new Notification
        {
            UserId = notification.CustomerId,
            Type = "BookingConfirmed",
            Title = "Booking Confirmed",
            Message = "Your booking has been confirmed.",
            ActionUrl = $"/bookings/{notification.BookingId}"
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _hubService.NotifyBookingConfirmedAsync(notification.BranchId, notification.BookingId, cancellationToken);
    }
}

public class SendBookingCancelledNotificationHandler : INotificationHandler<BookingCancelledEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IBookingHubService _hubService;
    private readonly IUserProfileService _userProfile;

    public SendBookingCancelledNotificationHandler(
        IApplicationDbContext context, IBookingHubService hubService, IUserProfileService userProfile)
    {
        _context = context;
        _hubService = hubService;
        _userProfile = userProfile;
    }

    public async Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
    {
        var ownerId = await _userProfile.GetOwnerIdByCenterIdAsync(notification.CenterId, cancellationToken);

        _context.Notifications.Add(new Notification
        {
            UserId = notification.CustomerId,
            Type = "BookingCancelled",
            Title = "Booking Cancelled",
            Message = notification.Reason ?? "Your booking has been cancelled.",
            ActionUrl = $"/bookings/{notification.BookingId}"
        });

        if (ownerId is not null)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = ownerId,
                Type = "BookingCancelled",
                Title = "Booking Cancelled",
                Message = notification.Reason ?? "A booking has been cancelled.",
                ActionUrl = $"/provider/bookings/{notification.BookingId}"
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _hubService.NotifyBookingCancelledAsync(notification.BranchId, notification.BookingId, notification.Reason, cancellationToken);
    }
}

public class SendReviewRequestNotificationHandler : INotificationHandler<BookingCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IFcmService _fcmService;
    private readonly IBookingHubService _hubService;
    private readonly IUserProfileService _userProfile;

    public SendReviewRequestNotificationHandler(
        IApplicationDbContext context, IFcmService fcmService, IBookingHubService hubService, IUserProfileService userProfile)
    {
        _context = context;
        _fcmService = fcmService;
        _hubService = hubService;
        _userProfile = userProfile;
    }

    public async Task Handle(BookingCompletedEvent notification, CancellationToken cancellationToken)
    {
        var fcmToken = await _userProfile.GetFcmTokenAsync(notification.CustomerId, cancellationToken);
        await _fcmService.SendAsync(fcmToken, "Rate Your Visit", "How was your experience? Leave a review!", cancellationToken: cancellationToken);

        _context.Notifications.Add(new Notification
        {
            UserId = notification.CustomerId,
            Type = "ReviewRequest",
            Title = "Rate Your Visit",
            Message = "Please share your experience with us.",
            ActionUrl = $"/bookings/{notification.BookingId}/review"
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _hubService.NotifyBookingCompletedAsync(notification.BranchId, notification.BookingId, cancellationToken);
    }
}
