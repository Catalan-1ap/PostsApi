using Core.Entities;


namespace Core.Interfaces;


public interface IIdentityService
{
    Task<string> Register(string userName, string email, string password);

    Task<User> Login(string email, string password);
}