public interface IDomainEventHandler
{
    bool CanHandle(object domainEvent);
    Task HandleAsync(object domainEvent, CancellationToken cancellationToken = default);
}