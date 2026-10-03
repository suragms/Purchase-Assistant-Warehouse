using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Infrastructure.Data.Configurations
{
    public class PurchaseDamageReportConfiguration : IEntityTypeConfiguration<PurchaseDamageReport>
    {
        public void Configure(EntityTypeBuilder<PurchaseDamageReport> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ItemName).IsRequired().HasMaxLength(500);
            builder.Property(e => e.Unit).HasMaxLength(32);
            builder.Property(e => e.DamageType).IsRequired().HasMaxLength(32);
            builder.Property(e => e.Reason).HasMaxLength(64);   // reference schema: String(64)
            builder.Property(e => e.Status).IsRequired().HasMaxLength(32);
            // The pending -> resolved transition is conditional; use the original status in
            // the UPDATE predicate so two reviewers cannot both resolve the same report.
            builder.Property(e => e.Status).IsConcurrencyToken();
            builder.Property(e => e.PhotoUrl).HasColumnType("text"); // reference: Text (unlimited)
            builder.Property(e => e.Notes).HasColumnType("text");    // reference: Text (unlimited)

            builder.Property(e => e.QtyDamaged).HasPrecision(18, 4);

            // Single-column FK for ReportedByUser only (Users are not tenant-scoped entities)
            builder.HasOne(e => e.ReportedByUser)
                .WithMany()
                .HasForeignKey(e => e.ReportedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // NOTE: PurchaseOrder and CatalogItem composite-key FKs are declared in
            // AppDbContext.ConfigureTenantRelationships to keep cross-tenant FK enforcement consistent.
            // Do NOT redeclare them here.

            builder.HasIndex(e => new { e.BusinessId, e.PurchaseOrderId });
            builder.HasIndex(e => new { e.BusinessId, e.Status });
            // Index for the ordered-by-CreatedAt list query
            builder.HasIndex(e => new { e.BusinessId, e.PurchaseOrderId, e.CreatedAt });
        }
    }
}
