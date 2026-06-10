namespace CleanArchitecture.Domain.Exceptions;

public class CancellationNotAllowedException : Exception
{
    public CancellationNotAllowedException() : base("Bookings can only be cancelled at least 24 hours before the appointment.") { }

    public CancellationNotAllowedException(string message) : base(message) { }
}
