namespace Core.Exceptions;


public sealed class NotFoundException : BusinessException
{
    public NotFoundException() : base(string.Empty) { }
}