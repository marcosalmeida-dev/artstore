namespace ArtStore.Shared.Events.Base;
public abstract class BaseDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public int TenantId { get; set; }
}