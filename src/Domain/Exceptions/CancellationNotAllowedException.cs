using System;

namespace CleanArchitecture.Domain.Exceptions;

public class CancellationNotAllowedException : BookingException
{
    public CancellationNotAllowedException()
        : base("Cancellation is not allowed within 24 hours of the booking time")
    {
    }

    public CancellationNotAllowedException(int hoursRemaining)
        : base($"Cancellation is not allowed. Only {hoursRemaining} hours remaining")
    {
    }
}
