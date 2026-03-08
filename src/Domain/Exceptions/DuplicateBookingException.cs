using System;

namespace CleanArchitecture.Domain.Exceptions;

public class DuplicateBookingException : BookingException
{
    public DuplicateBookingException()
        : base("A booking already exists for this time slot")
    {
    }
}
