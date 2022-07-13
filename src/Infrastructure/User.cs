using Domain;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure;


// TODO: allow duplicate username
public class User : IdentityUser, IUser { }