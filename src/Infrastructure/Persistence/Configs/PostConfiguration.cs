using Core.Entities;
using Core.StorageContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configs;


internal sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> post)
    {
        post.HasKey(x => x.Id);

        post.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(PostStorageContract.TitleMaxLength);

        post.Property(x => x.Body)
            .IsRequired()
            .HasMaxLength(PostStorageContract.BodyMaxLength);

        post.HasOne(x => x.Owner)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.OwnerId)
            .IsRequired();
    }
}