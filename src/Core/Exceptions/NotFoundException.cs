namespace Core.Exceptions;


public sealed class NotFoundException : BusinessException
{
    private NotFoundException(string message) : base(message) { }


    public static NotFoundException Make<T>(string entityName, T key) =>
        new($"Entity '{entityName}' with key ({key}) was not found.");
}