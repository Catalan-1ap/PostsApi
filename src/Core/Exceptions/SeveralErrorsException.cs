namespace Core.Exceptions;


public sealed class SeveralErrorsException : Exception
{

    public IEnumerable<string> Errors { get; }

    public SeveralErrorsException(IEnumerable<string> errors) => Errors = errors;
}