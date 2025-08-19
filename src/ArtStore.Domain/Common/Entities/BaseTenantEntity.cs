using ArtStore.Domain.Entities;

namespace ArtStore.Domain.Common.Entities;
public class BaseTenantEntity : BaseAuditableEntity //, ISoftDelete, IMayHaveTenant
{
    public int? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
