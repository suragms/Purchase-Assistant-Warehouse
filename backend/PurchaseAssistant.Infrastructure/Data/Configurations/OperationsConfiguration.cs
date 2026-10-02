using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Infrastructure.Data.Configurations
{
    public class StaffTaskConfiguration : IEntityTypeConfiguration<StaffTask>
    {
        public void Configure(EntityTypeBuilder<StaffTask> b)
        {
            b.Property(x => x.TaskType).HasMaxLength(64);
            b.Property(x => x.ReferenceId).HasMaxLength(255);
            b.Property(x => x.Status).HasMaxLength(16);
            b.Property(x => x.CorrectionNote).HasMaxLength(1000);
            b.Property(x => x.Version).IsConcurrencyToken();
            b.HasIndex(x => new { x.BusinessId, x.StaffId, x.Status, x.AssignedAt });
            b.HasOne<Membership>().WithMany().HasForeignKey(x => new { x.BusinessId, x.StaffId })
                .HasPrincipalKey(x => new { x.BusinessId, x.UserId }).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
        }
    }
    public class ChecklistTemplateConfiguration : IEntityTypeConfiguration<ChecklistTemplate>
    {
        public void Configure(EntityTypeBuilder<ChecklistTemplate> b)
        {
            b.Property(x => x.Key).HasMaxLength(64); b.Property(x => x.Slot).HasMaxLength(16); b.Property(x => x.Description).HasMaxLength(255);
            b.HasIndex(x => new { x.BusinessId, x.Slot, x.Key }).IsUnique();
        }
    }
    public class ChecklistCompletionConfiguration : IEntityTypeConfiguration<ChecklistCompletion>
    {
        public void Configure(EntityTypeBuilder<ChecklistCompletion> b)
        {
            b.Property(x => x.TaskKey).HasMaxLength(64); b.Property(x => x.Slot).HasMaxLength(16); b.Property(x => x.Notes).HasMaxLength(1000);
            b.HasIndex(x => new { x.BusinessId, x.CompletedByUserId, x.Date, x.Slot, x.TaskKey }).IsUnique();
            b.HasOne(x => x.CompletedByUser).WithMany().HasForeignKey(x => x.CompletedByUserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
    public class OperationsConfiguration
    {
        public class DailyUsageLogConfiguration : IEntityTypeConfiguration<DailyUsageLog>
        {
            public void Configure(EntityTypeBuilder<DailyUsageLog> builder)
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.OpeningQty).HasPrecision(18, 4);
                builder.Property(e => e.PurchasedQty).HasPrecision(18, 4);
                builder.Property(e => e.UsedQty).HasPrecision(18, 4);
                builder.Property(e => e.ClosingQty).HasPrecision(18, 4);
                builder.Property(e => e.Notes).HasColumnType("text");

                builder.HasOne(e => e.CatalogItem).WithMany()
                    .HasForeignKey(e => new { e.BusinessId, e.CatalogItemId })
                    .HasPrincipalKey(e => new { e.BusinessId, e.Id })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(e => e.LoggedByUser).WithMany()
                    .HasForeignKey(e => e.LoggedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasIndex(e => new { e.BusinessId, e.Date, e.CatalogItemId }).IsUnique();
            }
        }

        public class DailyOperationSnapshotConfiguration : IEntityTypeConfiguration<DailyOperationSnapshot>
        {
            public void Configure(EntityTypeBuilder<DailyOperationSnapshot> builder)
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.ChecklistCompletionRate).HasPrecision(6, 2);
                builder.Property(e => e.TotalQuantityUsed).HasPrecision(18, 4);

                builder.HasIndex(e => new { e.BusinessId, e.Date }).IsUnique();
            }
        }
    }
}
