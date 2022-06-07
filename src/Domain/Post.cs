namespace Domain;


public sealed class Post
{
    public Guid Id { get; private set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;


    public Post(string title, string body)
    {
        Title = title;
        Body = body;
    }


    // For EF
    private Post() { }
}