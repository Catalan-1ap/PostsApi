﻿using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configs;


internal sealed class DislikeConfiguration : IEntityTypeConfiguration<PostDislike>
{
    public void Configure(EntityTypeBuilder<PostDislike> like)
    {
        like.HasKey(x => new { x.PostId, x.UserId });

        like.HasOne(x => x.Post)
            .WithMany(x => x.Dislikes)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        like.HasOne(x => x.User)
            .WithMany(x => x.Dislikes)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}