using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configs;


internal sealed class LikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> like)
    {
        like.HasKey(x => new { x.PostId, x.UserId });

        like.HasOne(x => x.Post)
            .WithMany(x => x.Likes)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        like.HasOne(x => x.User)
            .WithMany(x => x.Likes)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}