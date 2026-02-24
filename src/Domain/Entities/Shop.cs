using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Entities;

public class Shop :BaseAuditableEntity
{
    public string? Name { get; set; }              // اسم المحل
    public string? Description { get; set; }       // وصف

    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    private bool _isOpen;
    public bool isOpen
    {
        get => _isOpen;
        set
        {
            if (value && !_isOpen)
            {
                AddDomainEvent(new ShipOpenEvent(this));
            }

            _isOpen = value;
        }
    }            // شغال ولا مقفول


}
