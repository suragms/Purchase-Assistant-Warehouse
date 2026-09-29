using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.ReferenceType)
                .HasMaxLength(100);

            // Indexes for optimizing dashboard/notification queries
            builder.HasIndex(n => new { n.BusinessId, n.UserId, n.IsRead });
            builder.HasIndex(n => new { n.BusinessId, n.UserId, n.CreatedAt });

            // Deduplication index: we want to quickly check if a specific notification already exists open for an entity
            // e.g., low stock for Item X.
            builder.HasIndex(n => new { n.BusinessId, n.UserId, n.Type, n.ReferenceId, n.IsRead });
        }
    }
}
