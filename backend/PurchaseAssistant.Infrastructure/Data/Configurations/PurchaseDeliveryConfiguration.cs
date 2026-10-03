using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;
namespace PurchaseAssistant.Infrastructure.Data.Configurations;
public class PurchaseDeliveryConfiguration : IEntityTypeConfiguration<PurchaseDelivery>
{
    public void Configure(EntityTypeBuilder<PurchaseDelivery> b)
    {
        b.ToTable("PurchaseDeliveries");
        b.HasIndex(x => new { x.BusinessId, x.PurchaseId }).IsUnique();
        b.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => new { x.BusinessId, x.PurchaseId }).HasPrincipalKey(x => new { x.BusinessId, x.Id }).OnDelete(DeleteBehavior.Restrict);
        b.Property(x => x.Version).IsConcurrencyToken();
        b.Property(x => x.Status).HasMaxLength(32);
        b.Property(x => x.RecipientLastFour).HasMaxLength(4);
        b.Property(x => x.MessageId).HasMaxLength(255);
        b.Property(x => x.ErrorCode).HasMaxLength(64);
    }
}
