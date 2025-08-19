using System.ComponentModel.DataAnnotations.Schema;
using ArtStore.Shared.Events.Base;

namespace ArtStore.Domain.Common.Entities;

public abstract class BaseEntity : IEntity<int>
{
    private readonly List<BaseDomainEvent> _domainEvents = new();

    [NotMapped] public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public virtual int Id { get; set; }

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