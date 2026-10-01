using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Infrastructure.Data.Configurations
{
    public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.MovementType).IsRequired().HasMaxLength(50);
            builder.Property(e => e.ReferenceType).HasMaxLength(50);
            builder.Property(e => e.ReferenceId).HasMaxLength(100);
            builder.Property(e => e.Reason).HasMaxLength(200);
            builder.Property(e => e.Notes).HasMaxLength(1000);

            // Precision for quantities
            builder.Property(e => e.QuantityDelta).HasPrecision(18, 4);
            builder.Property(e => e.QuantityBefore).HasPrecision(18, 4);
            builder.Property(e => e.QuantityAfter).HasPrecision(18, 4);

            builder.HasOne(e => e.CatalogItem)
                .WithMany()
                .HasForeignKey(e => e.CatalogItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.BusinessId, e.CatalogItemId, e.CreatedAt });
        }
    }
}
