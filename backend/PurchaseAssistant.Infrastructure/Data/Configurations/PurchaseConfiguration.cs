using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Infrastructure.Data.Configurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Version).IsRowVersion();

            builder.Property(e => e.OrderNumber).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Notes).HasMaxLength(1000);

            builder.Property(e => e.Subtotal).HasPrecision(18, 4);
            builder.Property(e => e.TaxTotal).HasPrecision(18, 4);
            builder.Property(e => e.GrandTotal).HasPrecision(18, 4);
            builder.Property(e => e.PaidAmount).HasPrecision(18, 4);

            builder.HasOne(e => e.Supplier)
                .WithMany()
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Broker)
                .WithMany()
                .HasForeignKey(e => e.BrokerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(e => e.Items)
                .WithOne(e => e.PurchaseOrder)
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.BusinessId, e.OrderNumber }).IsUnique();
            builder.HasIndex(e => new { e.BusinessId, e.Status });
        }
    }

    public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseItem> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.OrderedQuantity).HasPrecision(18, 4);
            builder.Property(e => e.ReceivedQuantity).HasPrecision(18, 4);
            builder.Property(e => e.UnitPrice).HasPrecision(18, 4);
            builder.Property(e => e.DiscountPercent).HasPrecision(6, 2);
            builder.Property(e => e.TaxPercent).HasPrecision(6, 2);
            builder.Property(e => e.KgPerUnit).HasPrecision(18, 4);
            builder.Property(e => e.LandingCostPerKg).HasPrecision(18, 4);
            builder.Property(e => e.LineTotal).HasPrecision(18, 4);
            builder.Property(e => e.Notes).HasMaxLength(500);

            builder.HasOne(e => e.CatalogItem)
                .WithMany()
                .HasForeignKey(e => e.CatalogItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.BusinessId, e.PurchaseOrderId });
        }
    }
}
