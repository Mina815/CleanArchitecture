using System;

namespace CleanArchitecture.Domain.Exceptions;

public class BranchClosedException : BookingException
{
    public BranchClosedException(DateOnly date)
        : base($"The branch is closed on {date}")
    {
    }
}
