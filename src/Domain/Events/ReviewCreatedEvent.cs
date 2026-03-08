using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Events;

public class ReviewCreatedEvent : BaseEvent
{
    public ReviewCreatedEvent(Review review)
    {
        Review = review;
    }

    public Review Review { get; }
}
