namespace Core.Entities;


public sealed class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string OwnerId { get; set; } = null!;
    public User Owner { get; set; } = null!;

    public ICollection<Like> Likes { get; set; } = null!;
    public ICollection<Dislike> Dislikes { get; set; } = null!;
}