﻿using System.Linq.Expressions;


namespace Core.Entities;


public sealed class Post : IAuditable
{
    public static readonly Expression<Func<Post, int>> RatingExpression = x => x.Likes.Count() - x.Dislikes.Count();

    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? CoverImageName { get; set; }
    public string? LeadBody { get; set; }
    public string Body { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string OwnerId { get; set; } = null!;
    public User Owner { get; set; } = null!;

    public ICollection<PostLike> Likes { get; set; } = null!;
    public ICollection<PostDislike> Dislikes { get; set; } = null!;
}