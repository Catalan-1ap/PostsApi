using Application.StorageContracts;
using Domain;
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
    }
}