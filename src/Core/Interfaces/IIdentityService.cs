using Core.Entities;


namespace Core.Interfaces;


public interface IIdentityService
{
    Task<User> RegisterAsync(string userName, string email, string password);

    Task<User> LoginAsync(string email, string password);

    Task AddToRoleAsync(User user, string role);

    Task<bool> IsUsernameUniqueAsync(string userName);

    Task<bool> IsEmailUniqueAsync(string email);

    Task<IList<string>> GetRolesAsync(User user);

    Task<bool> IsInRoleAsync(User user, string role);
}