using ArtStore.Shared.Events.Base;

namespace ArtStore.Domain.Common.Events;

public class UpdatedEvent<T> : BaseDomainEvent where T : IEntity
{
    public UpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}