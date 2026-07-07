using InventoryMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMS.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(supplier => supplier.SupplierId);
        builder.Property(supplier => supplier.SupplierName).HasMaxLength(100).IsRequired();
        builder.Property(supplier => supplier.ContactPerson).HasMaxLength(100).IsRequired();
        builder.Property(supplier => supplier.Phone).HasMaxLength(20).IsRequired();
        builder.Property(supplier => supplier.Email).HasMaxLength(150).IsRequired();
        builder.Property(supplier => supplier.Address).HasMaxLength(200).IsRequired();
        builder.HasIndex(supplier => supplier.Email).IsUnique();
    }
}

