using Microsoft.AspNetCore.Identity;


namespace Core.Entities;


public sealed class User : IdentityUser
{
    public List<Post> Posts { get; private set; } = new();
}