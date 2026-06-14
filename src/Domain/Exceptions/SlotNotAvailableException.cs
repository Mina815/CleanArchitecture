namespace CleanArchitecture.Domain.Exceptions;

public class SlotNotAvailableException : Exception
{
    public SlotNotAvailableException()
        : base("The requested time slot is not available.")
    {
    }

    public SlotNotAvailableException(string message)
        : base(message)
    {
    }
}
