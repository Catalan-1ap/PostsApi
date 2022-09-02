using Microsoft.AspNetCore.Identity;


namespace Core.Entities;


public sealed class User : IdentityUser
{
    public string? AvatarImageName { get; set; }

    public ICollection<Post> Posts { get; private set; } = null!;
    public ICollection<PostLike> Likes { get; private set; } = null!;
    public ICollection<PostDislike> Dislikes { get; private set; } = null!;
}