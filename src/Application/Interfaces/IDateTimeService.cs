﻿namespace Application.Interfaces;


public interface IDateTimeService
{
    DateTime UtcNow { get; }
    DateOnly UtcNowDate { get; }
}