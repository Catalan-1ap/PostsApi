﻿using Core.Interfaces;


namespace Infrastructure.Implementations;


public sealed class SystemDateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly UtcNowDate => DateOnly.FromDateTime(UtcNow);
}