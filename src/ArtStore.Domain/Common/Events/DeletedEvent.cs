using ArtStore.Shared.Events.Base;

namespace ArtStore.Domain.Common.Events;

public class DeletedEvent<T> : BaseDomainEvent where T : IEntity
{
    public DeletedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}