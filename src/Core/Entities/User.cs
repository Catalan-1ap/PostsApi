using Microsoft.AspNetCore.Identity;


namespace Core.Entities;


public sealed class User : IdentityUser
{
    public IReadOnlyList<Post> Posts { get; private set; } = null!;
}