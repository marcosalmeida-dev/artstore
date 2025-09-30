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
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<CouponUsage> CouponUsages { get; set; }
    public DbSet<InventoryLocation> InventoryLocations { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<InventoryReservation> InventoryReservations { get; set; }
    public DbSet<RecipeComponent> RecipeComponents { get; set; }
    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}