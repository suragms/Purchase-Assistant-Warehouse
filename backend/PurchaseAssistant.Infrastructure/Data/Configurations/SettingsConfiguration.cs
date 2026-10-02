using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseAssistant.Domain.Entities;
namespace PurchaseAssistant.Infrastructure.Data.Configurations;
public class SettingsConfiguration : IEntityTypeConfiguration<UserSettings>, IEntityTypeConfiguration<Business>, IEntityTypeConfiguration<ProviderCredential>
{
    public void Configure(EntityTypeBuilder<ProviderCredential> b)
    {
        b.Property(x => x.CredentialType).HasMaxLength(64);
        b.Property(x => x.LastFour).HasMaxLength(4);
        b.Property(x => x.Version).IsConcurrencyToken();
        b.HasIndex(x => new { x.BusinessId, x.CredentialType }).IsUnique();
        b.HasOne<Business>().WithMany().HasForeignKey(x => x.BusinessId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<User>().WithMany().HasForeignKey(x => x.UpdatedById).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<UserSettings> b)
    {
        b.HasIndex(x => new { x.BusinessId, x.UserId }).IsUnique();
        b.HasOne<Membership>().WithMany().HasForeignKey(x => new { x.BusinessId, x.UserId }).HasPrincipalKey(x => new { x.BusinessId, x.UserId }).OnDelete(DeleteBehavior.Cascade);
    }
    public void Configure(EntityTypeBuilder<Business> b)
    {
        b.Property(x => x.BrandingTitle).HasMaxLength(128); b.Property(x => x.BrandingLogoUrl).HasMaxLength(512);
        b.Property(x => x.GstNumber).HasMaxLength(20); b.Property(x => x.Address).HasMaxLength(2000); b.Property(x => x.Phone).HasMaxLength(32);
        b.Property(x => x.ContactEmail).HasMaxLength(255); b.Property(x => x.Version).IsConcurrencyToken().HasDefaultValueSql("gen_random_uuid()");
    }
}
