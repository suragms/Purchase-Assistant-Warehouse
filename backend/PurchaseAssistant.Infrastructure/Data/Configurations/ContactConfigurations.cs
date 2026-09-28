using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Infrastructure.Data.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);

            builder.HasIndex(c => new { c.BusinessId, c.Name }).IsUnique();
        }
    }

    public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
    {
        public void Configure(EntityTypeBuilder<Broker> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);

            builder.HasIndex(c => new { c.BusinessId, c.Name }).IsUnique();
        }
    }
}