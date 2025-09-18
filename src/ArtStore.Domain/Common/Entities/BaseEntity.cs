using System.ComponentModel.DataAnnotations.Schema;
using ArtStore.Shared.Events.Base;

namespace ArtStore.Domain.Common.Entities;

public abstract class BaseEntity<T> : IEntity<T>, IDomainEventEntity
{
    private readonly List<BaseDomainEvent> _domainEvents = new();

    [NotMapped] public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public virtual T Id { get; set; }

    public void AddDomainEvent(BaseDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}