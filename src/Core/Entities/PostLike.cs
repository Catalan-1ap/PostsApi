﻿namespace Core.Entities;


public sealed class PostLike
{
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}