using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Entities;

public class Review : BaseAuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid CenterId { get; set; }
    public Guid BookingId { get; set; }

    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public bool IsApproved { get; set; } = false;
    public DateTime? ApprovedAt { get; set; }

    // Navigation Properties
    //public User Customer { get; set; } = null!;
    public BeautyCenter Center { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
}
