using Core;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;


namespace Infrastructure.Implementations;


public sealed class DefaultStaticFilesService : IStaticFilesService
{
    private readonly CoreEnvironment _coreEnvironment;
    private readonly StaticResourcesEnvironment _staticResourcesEnvironment;


    public DefaultStaticFilesService(CoreEnvironment coreEnvironment, StaticResourcesEnvironment staticResourcesEnvironment)
    {
        _coreEnvironment = coreEnvironment;
        _staticResourcesEnvironment = staticResourcesEnvironment;
    }


    public async Task<string> SaveAvatarAsync(IFormFile avatarImage, string userId)
    {
        var fileName = $"{userId}{Path.GetExtension(avatarImage.FileName)}";
        var path = Path.Combine(_staticResourcesEnvironment.AvatarsFsPath, fileName);

        return await SaveStaticAsync(avatarImage, fileName, path);
    }


    public async Task<string> SaveCoverAsync(IFormFile coverImage, string postId)
    {
        var fileName = $"{postId}{Path.GetExtension(coverImage.FileName)}";
        var path = Path.Combine(_staticResourcesEnvironment.CoversFsPath, fileName);

        return await SaveStaticAsync(coverImage, fileName, path);
    }


    public void Remove(string uri)
    {
        var path = new Uri(uri).AbsolutePath.Replace('/', Path.DirectorySeparatorChar);
        var localpath = Path.Join(_staticResourcesEnvironment.StaticRootFsPath, path);

        File.Delete(localpath);
    }


    public string CreateAvatarUri(string fileName) => CreateUri(ApiRoutes.Static.Avatars, fileName);


    public string CreateCoverUri(string fileName) => CreateUri(ApiRoutes.Static.Covers, fileName);


    private async Task<string> SaveStaticAsync(IFormFile file, string fileName, string path)
    {
        await using var staticFile = new FileStream(
            path,
            new FileStreamOptions
            {
                Mode = FileMode.Create,
                Share = FileShare.None,
                Access = FileAccess.ReadWrite,
                Options = FileOptions.Asynchronous
            }
        );

        await file.CopyToAsync(staticFile);

        return fileName;
    }


    private string CreateUri(string requestPath, string fileName) =>
        new Uri(_coreEnvironment.BaseUrl, $"{requestPath}/{fileName}").ToString();
}