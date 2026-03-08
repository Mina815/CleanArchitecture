using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Entities;

public class BeautyCenter : BaseAuditableEntity
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; } = false;
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;

    //// Navigation Properties
    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<CenterImage> Images { get; set; } = new List<CenterImage>();
}
