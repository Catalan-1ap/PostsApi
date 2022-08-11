using Core.Entities;
using Core.Interfaces;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;


namespace Api.Endpoints.Posts.Common;


public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull, new()
    where TResponse : notnull, new()
{
    public abstract IIdentityService IdentityService { get; init; }
    public abstract IApplicationDbContext ApplicationDbContext { get; init; }


    public async Task<Post?> PostShouldExistsAsync(Guid id, CancellationToken ct)
    {
        var post = await ApplicationDbContext.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return post;
    }


    protected bool IsOwner(Post post, string userId) => post.OwnerId == userId;


    protected async Task<bool> IsAdminAsync(DeleteRequest req) =>
        await IdentityService.IsInRoleAsync(new() { Id = req.UserId }, Role.Admin);
}