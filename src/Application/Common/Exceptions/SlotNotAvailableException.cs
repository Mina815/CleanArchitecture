namespace CleanArchitecture.Application.Common.Exceptions;

public class SlotNotAvailableException : Exception
{
    public SlotNotAvailableException(string message) : base(message)
    {
    }
}
