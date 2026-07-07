using InventoryMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMS.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.ProductId);
        builder.Property(product => product.ProductName).HasMaxLength(100).IsRequired();
        builder.Property(product => product.ProductDesc).HasMaxLength(250).IsRequired();
        builder.Property(product => product.ProductPrice).HasPrecision(18,2).IsRequired();
        builder.Property(product => product.QuantityInStock).IsRequired();
        builder.Property(product => product.CreatedAt).IsRequired();

        builder.HasOne(product => product.Supplier)
            .WithMany(supplier => supplier.Products)
            .HasForeignKey(product => product.SupplierId)
            .OnDelete(DeleteBehavior.Restrict); //Supplier with any product can't be deleted

        builder.HasIndex(product => product.ProductName).IsUnique();
    }
}