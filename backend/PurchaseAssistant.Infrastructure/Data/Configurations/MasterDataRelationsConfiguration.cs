using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Infrastructure.Data.Configurations
{
    public class MasterDataRelationsConfiguration : IEntityTypeConfiguration<CatalogVariant>
    {
        public void Configure(EntityTypeBuilder<CatalogVariant> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(512);
            builder.Property(x => x.KgPerUnit).HasPrecision(18, 4);
            builder.Property(x => x.RowVersion).IsConcurrencyToken().HasDefaultValueSql("gen_random_uuid()");
            builder.Property<string>("NormalizedName").HasComputedColumnSql("lower(btrim(\"Name\"))", stored: true);
            builder.HasIndex("BusinessId", "CatalogItemId", "NormalizedName").IsUnique();
            builder.HasIndex(x => new { x.BusinessId, x.CatalogItemId });
        }
    }

    public class SupplierItemConfiguration : IEntityTypeConfiguration<SupplierItem>
    {
        public void Configure(EntityTypeBuilder<SupplierItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.BusinessId, x.SupplierId, x.CatalogItemId }).IsUnique();
        }
    }

    public class BrokerSupplierConfiguration : IEntityTypeConfiguration<BrokerSupplier>
    {
        public void Configure(EntityTypeBuilder<BrokerSupplier> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.BusinessId, x.BrokerId, x.SupplierId }).IsUnique();
        }
    }

    public class SupplierItemPriceConfiguration : IEntityTypeConfiguration<SupplierItemPrice>
    {
        public void Configure(EntityTypeBuilder<SupplierItemPrice> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Price).HasPrecision(18, 4);
            builder.Property(x => x.PricePerKg).HasPrecision(18, 4);
            builder.HasIndex(x => new { x.BusinessId, x.SupplierId, x.CatalogItemId });
        }
    }
}
