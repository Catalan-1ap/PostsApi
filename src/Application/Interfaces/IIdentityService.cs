namespace Application.Interfaces;


public interface IIdentityService
{
    Task<string> Register(string userName, string email, string password);
}