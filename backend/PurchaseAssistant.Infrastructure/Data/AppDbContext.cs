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
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<PurchaseOrder> Purchases => Set<PurchaseOrder>();
        public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<PurchaseDamageReport> PurchaseDamageReports => Set<PurchaseDamageReport>();

        public Guid CurrentBusinessId => _tenantProvider?.GetBusinessId() ?? Guid.Empty;

        private void ProtectStockLedger()
        {
            if (ChangeTracker.Entries<StockMovement>().Any(e => e.State is EntityState.Modified or EntityState.Deleted))
                throw new InvalidOperationException("Stock movements are immutable. Record a correcting movement instead.");
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ProtectStockLedger();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ProtectStockLedger();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            ConfigureTenantRelationships(modelBuilder);

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
            modelBuilder.Entity<StockMovement>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<PurchaseOrder>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<PurchaseItem>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<Notification>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
            modelBuilder.Entity<PurchaseDamageReport>().HasQueryFilter(e => e.BusinessId == CurrentBusinessId);
        }

        private static void ConfigureTenantRelationships(ModelBuilder model)
        {
            // Include business in relational keys so raw/direct EF writes cannot link another tenant's row.
            model.Entity<CategoryType>().HasOne(e => e.Category).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CategoryId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<CatalogItem>().HasOne(e => e.Category).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CategoryId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<CatalogItem>().HasOne(e => e.Type).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CategoryId, e.TypeId }).HasPrincipalKey(e => new { e.BusinessId, e.CategoryId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<CatalogItem>().HasOne(e => e.LastSupplier).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.LastSupplierId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<CatalogItem>().HasOne(e => e.LastBroker).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.LastBrokerId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<CatalogVariant>().HasOne(e => e.CatalogItem).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CatalogItemId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<SupplierItem>().HasOne(e => e.Supplier).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.SupplierId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<SupplierItem>().HasOne(e => e.CatalogItem).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CatalogItemId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<BrokerSupplier>().HasOne(e => e.Broker).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.BrokerId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<BrokerSupplier>().HasOne(e => e.Supplier).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.SupplierId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<SupplierItemPrice>().HasOne(e => e.Supplier).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.SupplierId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<SupplierItemPrice>().HasOne(e => e.CatalogItem).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CatalogItemId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<SupplierItemPrice>().HasOne<PurchaseOrder>().WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.SourcePurchaseId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<PurchaseOrder>().HasOne(e => e.Supplier).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.SupplierId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<PurchaseOrder>().HasOne(e => e.Broker).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.BrokerId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<PurchaseOrder>().HasMany(e => e.Items).WithOne(e => e.PurchaseOrder)
                .HasForeignKey(e => new { e.BusinessId, e.PurchaseOrderId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Cascade);
            model.Entity<PurchaseItem>().HasOne(e => e.CatalogItem).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CatalogItemId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<StockMovement>().HasOne(e => e.CatalogItem).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CatalogItemId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Restrict);
            model.Entity<PurchaseDamageReport>().HasOne(e => e.PurchaseOrder).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.PurchaseOrderId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.Cascade);
            model.Entity<PurchaseDamageReport>().HasOne(e => e.CatalogItem).WithMany()
                .HasForeignKey(e => new { e.BusinessId, e.CatalogItemId }).HasPrincipalKey(e => new { e.BusinessId, e.Id }).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
