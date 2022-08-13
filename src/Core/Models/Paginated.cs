namespace Core.Models;


public class Paginated<T>
{
    public int TotalPages { get; init; }
    public int Page { get; set; }
    public int PageSize { get; init; }
    public ICollection<T> Data { get; init; } = null!;
}