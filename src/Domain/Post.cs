namespace Domain;


public sealed class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
}