using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Events;

public class BookingCancelledEvent : BaseEvent
{
    public BookingCancelledEvent(Booking booking, string reason)
    {
        Booking = booking;
        Reason = reason;
    }

    public Booking Booking { get; }
    public string Reason { get; }
}
