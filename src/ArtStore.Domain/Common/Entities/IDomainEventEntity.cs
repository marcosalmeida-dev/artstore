using ArtStore.Shared.Events.Base;

namespace ArtStore.Domain.Common.Entities;

public interface IDomainEventEntity
{
    IReadOnlyCollection<BaseDomainEvent> DomainEvents { get; }
    void AddDomainEvent(BaseDomainEvent domainEvent);
    void RemoveDomainEvent(BaseDomainEvent domainEvent);
    void ClearDomainEvents();
}