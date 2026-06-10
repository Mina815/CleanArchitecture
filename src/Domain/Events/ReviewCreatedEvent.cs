namespace CleanArchitecture.Domain.Events;

public class ReviewCreatedEvent : BaseEvent
{
    public ReviewCreatedEvent(int centerId, int rating)
    {
        CenterId = centerId;
        Rating = rating;
    }

    public int CenterId { get; }
    public int Rating { get; }
}
