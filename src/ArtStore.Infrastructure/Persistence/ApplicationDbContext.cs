// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using ArtStore.Domain.Common.Entities;
using ArtStore.Domain.Identity;
using ArtStore.Infrastructure.Persistence.Configurations;
using ArtStore.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace ArtStore.Infrastructure.Persistence;

#nullable disable
public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, string,
    ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
    ApplicationRoleClaim, ApplicationUserToken>, IApplicationDbContext, IDataProtectionKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    //public DbSet<SystemLog> SystemLogs { get; set; }
    //public DbSet<AuditTrail> AuditTrails { get; set; }
    //public DbSet<Document> Documents { get; set; }
    //public DbSet<PicklistSet> PicklistSets { get; set; }
    //public DbSet<Contact> Contacts { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);

        builder.ApplyConfiguration(new TenantConfiguration());
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new ProductConfiguration());
        builder.ApplyConfiguration(new ProductImageConfiguration());
        builder.ApplyConfiguration(new OrderConfiguration());
        builder.ApplyConfiguration(new OrderDetailConfiguration());
        builder.ApplyConfiguration(new OrderStatusHistoryConfiguration());
        builder.ApplyConfiguration(new CouponConfiguration());
        builder.ApplyConfiguration(new CouponUsageConfiguration());
        builder.ApplyConfiguration(new InventoryLocationConfiguration());
        builder.ApplyConfiguration(new InventoryItemConfiguration());
        builder.ApplyConfiguration(new StockMovementConfiguration());
        builder.ApplyConfiguration(new InventoryReservationConfiguration());
        builder.ApplyConfiguration(new RecipeComponentConfiguration());
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<string>().HaveMaxLength(450);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
}