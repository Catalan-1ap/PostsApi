using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace Infrastructure.Persistence.Interceptors;


public class AuditableSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeService _dateTimeService;


    public AuditableSaveChangesInterceptor(IDateTimeService dateTimeService) => _dateTimeService = dateTimeService;


    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is null)
            return ValueTask.FromResult(result);

        foreach (var auditable in eventData.Context.ChangeTracker.Entries<IAuditable>())
            switch (auditable.State)
            {
                case EntityState.Added:
                    auditable.Entity.CreatedAt = _dateTimeService.UtcNow;
                    break;
                case EntityState.Modified:
                    auditable.Entity.UpdatedAt = _dateTimeService.UtcNow;
                    break;
            }

        return ValueTask.FromResult(result);
    }
}