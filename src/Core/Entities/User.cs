using Microsoft.AspNetCore.Identity;


namespace Core.Entities;


public sealed class User : IdentityUser
{
    public ICollection<Post> Posts { get; private set; } = null!;
    public ICollection<Like> Likes { get; private set; } = null!;
    public ICollection<Dislike> Dislikes { get; private set; } = null!;
}