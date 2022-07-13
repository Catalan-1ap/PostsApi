namespace Application.Exceptions;


public sealed class SeveralErrorsException : Exception
{

    public SeveralErrorsException(IEnumerable<string> errors) => Errors = errors;

    public IEnumerable<string> Errors { get; }
}