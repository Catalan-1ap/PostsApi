using Microsoft.AspNetCore.Http;


namespace Core.Interfaces;


public interface IStaticFilesService
{
    Task<string> SaveAvatarAsync(IFormFile avatarImage, string userId);

    string CreateAvatarUri(string fileName);

    Task<string> SaveCoverAsync(IFormFile coverImage, string postId);

    string CreateCoverUri(string fileName);

    void Remove(string uri);
}