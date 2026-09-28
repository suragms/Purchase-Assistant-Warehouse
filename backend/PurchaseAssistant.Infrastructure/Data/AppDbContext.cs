using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using System;
using System.Reflection;

namespace PurchaseAssistant.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantProvider? _tenantProvider;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider? tenantProvider = null) : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Business> Businesses => Set<Business>();
        public DbSet<Membership> Memberships => Set<Membership>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CategoryType> CategoryTypes => Set<CategoryType>();
        public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
        public DbSet<CatalogVariant> CatalogVariants => Set<CatalogVariant>();
        public DbSet<SupplierItem> SupplierItems => Set<SupplierItem>();
        public DbSet<BrokerSupplier> BrokerSuppliers => Set<BrokerSupplier>();
        public DbSet<SupplierItemPrice> SupplierItemPrices => Set<SupplierItemPrice>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Broker> Brokers => Set<Broker>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<SecurityAuditLog> SecurityAuditLogs => Set<SecurityAuditLog>();

        public Guid CurrentBusinessId => _tenantProvider?.GetBusinessId() ?? Guid.Empty;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Multi-tenant Query Filters
            modelBuilder.Entity<Category>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<CategoryType>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<CatalogItem>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<CatalogVariant>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<SupplierItem>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<BrokerSupplier>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<SupplierItemPrice>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<Supplier>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<Broker>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<SecurityAuditLog>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
        }
    }
}