using Core.Entities;


namespace Core.Interfaces;


public interface IIdentityService
{
    Task<User> RegisterAsync(string userName, string email, string password);

    Task<User> LoginAsync(string email, string password);

    Task<User> GetByIdAsync(string id);

    Task<bool> IsUsernameUniqueAsync(string userName);

    Task<bool> IsEmailUniqueAsync(string email);
}