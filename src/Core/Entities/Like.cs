namespace Core.Entities;


public sealed class Like
{
    public Guid PostId { get; set; }
    public Post Post { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}