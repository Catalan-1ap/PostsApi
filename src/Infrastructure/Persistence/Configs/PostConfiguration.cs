using Core.Entities;
using Core.StorageContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configs;


internal sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> post)
    {
        post.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(PostStorageContract.TitleMaxLength);

        post.Property(p => p.Body)
            .IsRequired()
            .HasMaxLength(PostStorageContract.BodyMaxLength);

        post.HasOne(x => x.Owner)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.OwnerId)
            .IsRequired();
    }
}