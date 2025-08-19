using ArtStore.Shared.Events.Base;

namespace ArtStore.Domain.Common.Events;

public class CreatedEvent<T> : BaseDomainEvent where T : IEntity
{
    public CreatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}