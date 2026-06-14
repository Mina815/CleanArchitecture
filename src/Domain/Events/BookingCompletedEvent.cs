namespace CleanArchitecture.Domain.Events;

public class BookingCompletedEvent : BaseEvent
{
    public BookingCompletedEvent(Booking booking)
    {
        Booking = booking;
    }

    public Booking Booking { get; }
}
