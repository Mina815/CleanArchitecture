using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Domain.Entities;

public class Review : BaseAuditableEntity
{
    public string CustomerId { get; set; } = null!;

    public int CenterId { get; set; }

    public BeautyCenter Center { get; set; } = null!;

    public int BookingId { get; set; }

    public Booking Booking { get; set; } = null!;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsApproved { get; set; } = true;

    public DateTimeOffset? ApprovedAt { get; set; }

    public static Review Create(string customerId, int centerId, int bookingId, int rating, string? comment)
    {
        var review = new Review
        {
            CustomerId = customerId,
            CenterId = centerId,
            BookingId = bookingId,
            Rating = rating,
            Comment = comment,
            IsApproved = true,
            ApprovedAt = DateTimeOffset.UtcNow
        };

        review.AddDomainEvent(new ReviewCreatedEvent(centerId, rating));
        return review;
    }
}
