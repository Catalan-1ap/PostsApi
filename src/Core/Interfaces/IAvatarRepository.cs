using Microsoft.AspNetCore.Http;


namespace Core.Interfaces;


public interface IAvatarRepository
{
    Task<string> Save(IFormFile avatarFile, string userId);
}