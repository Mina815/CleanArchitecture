using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Events;

public class BookingConfirmedEvent : BaseEvent
{
    public BookingConfirmedEvent(Booking booking)
    {
        Booking = booking;
    }

    public Booking Booking { get; }
}
