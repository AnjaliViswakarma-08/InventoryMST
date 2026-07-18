using InventoryMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMS.Configurations;


public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(orderItem => orderItem.OrderItemId);
        builder.Property(orderItem => orderItem.UnitPrice).HasPrecision(18,2);
        builder.Property(orderItem => orderItem.TotalPrice).HasPrecision(18,2);

        builder.HasOne(orderItem => orderItem.Order)
            .WithMany(order => order.OrderItems)
            .HasForeignKey(orderItem => orderItem.OrderId)
            .OnDelete(DeleteBehavior.Cascade); //Deleting an order will delete all its order items

        builder.HasOne(orderItem => orderItem.Product)
        .WithMany(product => product.OrderItems)
        .HasForeignKey(orderItem => orderItem.ProductId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
