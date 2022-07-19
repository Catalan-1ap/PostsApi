namespace Core.Entities;


public sealed class Post
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;

    public string OwnerId { get; set; } = null!;
    public User Owner { get; private set; } = null!;


    public Post(string title, string body)
    {
        Title = title;
        Body = body;
    }


    private Post()
    {
        // required by EF
    }
}