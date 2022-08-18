using Core;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;


namespace Infrastructure.Services;


public sealed class DefaultStaticFilesService : IStaticFilesService
{
    private readonly CoreEnvironment _environment;


    public DefaultStaticFilesService(CoreEnvironment environment) =>
        _environment = environment;


    public async Task<string> SaveAvatar(IFormFile avatarImage, string userId)
    {
        var fileName = $"{userId}{Path.GetExtension(avatarImage.FileName)}";
        var path = Path.Combine(_environment.AvatarsFsPath, fileName);

        return await SaveStatic(avatarImage, fileName, path);
    }


    public async Task<string> SaveCover(IFormFile coverImage, string postId)
    {
        var fileName = $"{postId}{Path.GetExtension(coverImage.FileName)}";
        var path = Path.Combine(_environment.CoversFsPath, fileName);

        return await SaveStatic(coverImage, fileName, path);
    }


    public void Remove(string uri)
    {
        var path = new Uri(uri).AbsolutePath.Replace('/', Path.DirectorySeparatorChar);
        var localpath = Path.Join(_environment.StaticRootFsPath, path);

        File.Delete(localpath);
    }


    public string CreateAvatarUri(string fileName) => CreateUri(ApiRoutes.Static.Avatars, fileName);


    public string CreateCoverUri(string fileName) => CreateUri(ApiRoutes.Static.Covers, fileName);


    private async Task<string> SaveStatic(IFormFile file, string fileName, string path)
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
        new Uri(_environment.BaseUri, $"{requestPath}/{fileName}").ToString();
}