using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ArtStore.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    //DbSet<SystemLog> SystemLogs { get; set; }
    //DbSet<AuditTrail> AuditTrails { get; set; }
    //DbSet<Document> Documents { get; set; }
    //DbSet<PicklistSet> PicklistSets { get; set; }
    //DbSet<Product> Products { get; set; }
    //DbSet<Tenant> Tenants { get; set; }
    //DbSet<Contact> Contacts { get; set; }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}