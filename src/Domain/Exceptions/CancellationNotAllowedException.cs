namespace CleanArchitecture.Domain.Exceptions;

public class CancellationNotAllowedException : Exception
{
    public CancellationNotAllowedException()
        : base("Bookings cannot be cancelled within 24 hours of the appointment.")
    {
    }

    public CancellationNotAllowedException(string message)
        : base(message)
    {
    }
}
