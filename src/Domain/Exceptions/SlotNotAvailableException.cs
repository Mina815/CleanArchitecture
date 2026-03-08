using System;

namespace CleanArchitecture.Domain.Exceptions;

public class SlotNotAvailableException : BookingException
{
    public SlotNotAvailableException()
        : base("The selected time slot is not available")
    {
    }

    public SlotNotAvailableException(DateOnly date, TimeOnly time)
        : base($"The time slot on {date} at {time} is not available")
    {
    }
}
