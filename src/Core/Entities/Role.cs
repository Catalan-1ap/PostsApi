using Microsoft.AspNetCore.Identity;


namespace Core.Entities;


public sealed class Role : IdentityRole
{
    public static string Admin => "Admin";
}