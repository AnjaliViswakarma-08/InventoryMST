using InventoryMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMS.Configurations;

public sealed class OtpEntryConfiguration : IEntityTypeConfiguration<OtpEntry>
{
    public void Configure(EntityTypeBuilder<OtpEntry> builder)
    {
        builder.HasKey(e => e.OtpEntryId);
        builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
        builder.Property(e => e.CodeHash).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Purpose).HasMaxLength(50).IsRequired();
        builder.Property(e => e.ExpiresAtUtc).IsRequired();
        builder.Property(e => e.IsUsed).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.CreatedAtUtc).IsRequired();
        builder.HasIndex(e => new { e.Email, e.Purpose });
    }
}
