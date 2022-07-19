using Microsoft.AspNetCore.Identity;


namespace Core.Entities;


public sealed class Role : IdentityRole
{
    public Role(string id, string roleName)
    {
        Id = id;
        Name = roleName;
    }


    public Role(string roleName) : this(Guid.NewGuid().ToString(), roleName) { }


    private Role()
    {
        // required by EF
    }
}