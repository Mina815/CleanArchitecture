namespace CleanArchitecture.Application.Common.Exceptions;

public class CancellationNotAllowedException : Exception
{
    public CancellationNotAllowedException(string message) : base(message)
    {
    }
}
