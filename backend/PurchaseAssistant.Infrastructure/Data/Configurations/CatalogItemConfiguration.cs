using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Infrastructure.Data.Configurations
{
    public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.HasKey(c => c.Id);

            // Decimal precision
            builder.Property(c => c.KgPerUnit).HasPrecision(18, 4);
            builder.Property(c => c.ReorderLevel).HasPrecision(18, 4);
            builder.Property(c => c.CurrentStock).HasPrecision(18, 4);
            builder.Property(c => c.PhysicalStock).HasPrecision(18, 4);
            builder.Property(c => c.ReservedStock).HasPrecision(18, 4);

            // Concurrency token
            builder.Property(c => c.RowVersion).IsConcurrencyToken();

            builder.Property(c => c.ItemCode).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Barcode).HasMaxLength(100);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);

            // Constraints per business
            builder.HasIndex(c => new { c.BusinessId, c.ItemCode }).IsUnique();
            builder.HasIndex(c => new { c.BusinessId, c.Barcode }).IsUnique().HasFilter("\"Barcode\" IS NOT NULL");
        }
    }
}