using Core.Interfaces;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;


namespace Infrastructure.Services;


public sealed class PhysicalAvatarRepository : IAvatarRepository
{
    private readonly IHostEnvironment _environment;


    public PhysicalAvatarRepository(IHostEnvironment environment) => _environment = environment;


    public async Task<string> Save(IFormFile avatarFile, string userId)
    {
        var path = _environment.FsAvatarsPath();
        var fileName = $"{userId}{Path.GetExtension(avatarFile.FileName)}";

        await using var staticFile = new FileStream(
            Path.Combine(path, fileName),
            new FileStreamOptions
            {
                Mode = FileMode.Create,
                Share = FileShare.None,
                Access = FileAccess.ReadWrite,
                Options = FileOptions.Asynchronous
            }
        );

        await avatarFile.CopyToAsync(staticFile);

        return fileName;
    }
}