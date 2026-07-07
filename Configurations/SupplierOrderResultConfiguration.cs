using InventoryMS.Models.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMS.Configurations;

public sealed class SupplierOrderReportResultConfiguration : IEntityTypeConfiguration<SupplierOrderResult>
{
    public void Configure(EntityTypeBuilder<SupplierOrderResult> builder)
    {
        builder.HasNoKey();
        builder.ToView(null);
        builder.Property(result => result.SupplierName).HasMaxLength(150);
        builder.Property(result => result.TotalOrderValue).HasPrecision(18, 2);
    }
}
