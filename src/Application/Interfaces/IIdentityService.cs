using Domain;


namespace Application.Interfaces;


public interface IIdentityService
{
    Task<string> Register(string userName, string email, string password);

    Task<IUser> Login(string email, string password);

    Task<IUser> CurrentUser();
}