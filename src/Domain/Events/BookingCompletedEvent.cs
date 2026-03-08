using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Events;

public class BookingCompletedEvent : BaseEvent
{
    public BookingCompletedEvent(Booking booking)
    {
        Booking = booking;
    }

    public Booking Booking { get; }
}
