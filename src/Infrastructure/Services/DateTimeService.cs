using Application.Interfaces;


namespace Infrastructure.Services;


public sealed class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly UtcNowDate => DateOnly.FromDateTime(UtcNow);
}