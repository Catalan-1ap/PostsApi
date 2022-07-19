using Microsoft.AspNetCore.Identity;


namespace Core.Entities;


public sealed class User : IdentityUser
{
    public List<Post> Posts { get; private set; } = new();


    public User(string id, string userName, string email)
    {
        Id = id;
        UserName = userName;
        Email = email;
    }


    public User(string userName, string email) : this(Guid.NewGuid().ToString(), userName, email) { }


    private User()
    {
        // required by EF 
    }
}