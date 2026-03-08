using System;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Exceptions;

public class InvalidBookingStatusException : BookingException
{
    public InvalidBookingStatusException(BookingStatus currentStatus, BookingStatus attemptedStatus)
        : base($"Cannot change booking status from {currentStatus} to {attemptedStatus}")
    {
    }
}
