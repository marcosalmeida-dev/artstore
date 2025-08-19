namespace ArtStore.Domain.Common.Events.Dispatcher;

public interface IDomainEventDispatcher
{
    Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default);
}