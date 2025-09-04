namespace ArtStore.Shared.Events.Base;
public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
    int TenantId { get; }
}