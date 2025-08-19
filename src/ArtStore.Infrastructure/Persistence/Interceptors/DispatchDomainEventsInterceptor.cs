using ArtStore.Domain.Common.Entities;
using ArtStore.Domain.Common.Events.Dispatcher;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ArtStore.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor for dispatching domain events when saving changes in the database.
/// </summary>
public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{   
    private readonly IDomainEventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatchDomainEventsInterceptor"/> class.
    /// </summary>
    /// <param name="eventDispatcher">The domain event dispatcher instance.</param>
    public DispatchDomainEventsInterceptor(IDomainEventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc/>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var domainEventEntities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any() && e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = domainEventEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        if (domainEvents.Any())
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var saveResult = await base.SavingChangesAsync(eventData, result, cancellationToken);

                domainEventEntities.ForEach(e => e.ClearDomainEvents());
                foreach (var domainEvent in domainEvents)
                {
                    await _eventDispatcher.PublishAsync(domainEvent, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return saveResult;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var domainEventEntities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any() && e.State != EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = domainEventEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        if (domainEvents.Any())
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var saveResult = await base.SavedChangesAsync(eventData, result, cancellationToken);

                domainEventEntities.ForEach(e => e.ClearDomainEvents());
                foreach (var domainEvent in domainEvents)
                {
                    await _eventDispatcher.PublishAsync(domainEvent, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return saveResult;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
