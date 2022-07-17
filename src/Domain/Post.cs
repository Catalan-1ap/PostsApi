namespace Domain;


public sealed class Post
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }


    public Post(string title, string body)
    {
        Title = title;
        Body = body;
    }
}