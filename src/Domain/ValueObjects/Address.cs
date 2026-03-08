using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.ValueObjects;

public record Address
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }

    public Address() { }

    public Address(string street, string city, string district, decimal latitude, decimal longitude)
    {
        Street = street;
        City = city;
        District = district;
        Latitude = latitude;
        Longitude = longitude;
    }

    public string GetFullAddress()
    {
        return $"{Street}, {District}, {City}";
    }
}
