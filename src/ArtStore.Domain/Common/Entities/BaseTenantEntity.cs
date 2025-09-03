using ArtStore.Domain.Entities;

namespace ArtStore.Domain.Common.Entities;
public class BaseTenantEntity<T> : BaseAuditableEntity<T>, ISoftDelete
{
    public int? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsActive { get; set; } = true;
}
