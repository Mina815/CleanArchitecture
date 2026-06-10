namespace CleanArchitecture.Domain.Exceptions;

public class SlotNotAvailableException : Exception
{
    public SlotNotAvailableException() : base("The selected time slot is not available.") { }

    public SlotNotAvailableException(string message) : base(message) { }
}
