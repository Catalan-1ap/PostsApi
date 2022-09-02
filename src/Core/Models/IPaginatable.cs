namespace Core.Models;


public interface IPaginatable
{
    public int Page { get; }
    public int PageSize { get; }
}