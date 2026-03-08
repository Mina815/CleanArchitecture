using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CleanArchitecture.Domain.Entities;

public class Branch : BaseAuditableEntity
{
    public Guid CenterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? WhatsAppNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public BeautyCenter Center { get; set; } = null!;
    public ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
    public ICollection<Staff> Staff { get; set; } = new List<Staff>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<TimeOff> TimeOffs { get; set; } = new List<TimeOff>();
}
