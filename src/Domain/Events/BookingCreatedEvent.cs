using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Events;

public class BookingCreatedEvent : BaseEvent
{
    public BookingCreatedEvent(Booking booking)
    {
        Booking = booking;
    }

    public Booking Booking { get; }
}
