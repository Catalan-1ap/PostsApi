namespace Core.Models;


public interface IPaginatable
{
    public int Page { get; init; }
    public int PageSize { get; init; }
}