using Microsoft.AspNetCore.Http;


namespace Core.Interfaces;


public interface IStaticFilesService
{
    Task<string> SaveAvatar(IFormFile avatarImage, string userId);

    string CreateAvatarUri(string fileName);

    Task<string> SaveCover(IFormFile coverImage, string postId);

    string CreateCoverUri(string fileName);

    void Remove(string uri);
}