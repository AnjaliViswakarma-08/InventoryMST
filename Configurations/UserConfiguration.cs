using InventoryMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMS.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.UserId);
        builder.Property(user => user.Firstname).HasMaxLength(50).IsRequired();
        builder.Property(user => user.Lastname).HasMaxLength(50).IsRequired();
        builder.Property(user => user.Gender).HasMaxLength(20).IsRequired();
        builder.Property(user => user.Address).HasMaxLength(250).IsRequired();
        builder.Property(user => user.Phone).HasMaxLength(20).IsRequired();
        builder.Property(user => user.Email).HasMaxLength(200).IsRequired();
        builder.Property(user => user.Role).HasMaxLength(50).IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(user => user.CreatedAt).IsRequired();
        builder.HasIndex(user => user.Email).IsUnique(); //unique index for email(every user should have unique email)
        builder.HasIndex(user => user.Phone).IsUnique();
    }
}