using InventoryMS.Helpers;
using InventoryMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        await dbContext.Database.MigrateAsync();

        // Deploy the stored procedure for reporting
        await dbContext.Database.ExecuteSqlRawAsync(@"
CREATE OR ALTER PROCEDURE dbo.sp_GetSupplierOrderReport
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.SupplierName,
        COALESCE(SUM(oi.Quantity), 0) AS TotalQuantityOrdered,
        COALESCE(SUM(oi.TotalPrice), 0) AS TotalOrderValue
    FROM Suppliers AS s
    LEFT JOIN Products AS p ON p.SupplierId = s.SupplierId
    LEFT JOIN OrderItems AS oi ON oi.ProductId = p.ProductId
    GROUP BY s.SupplierName
    ORDER BY s.SupplierName;
END;");

        if (!await dbContext.Users.AnyAsync())
        {
            var firstUser = new User
            {
                Firstname = "Aman",
                Lastname = "Gupta",
                Age = 30,
                Gender = "Male",
                Address = "Nayapalli",
                Phone = "+10000000001",
                Email = "owner@ims.local",
                Role = RoleName.Owner,
                CreatedAt = DateTime.UtcNow
            };
            firstUser.PasswordHash = passwordHasher.HashPassword(firstUser, "Admin@123");

            var secUser = new User
            {
                Firstname = "Surabhi",
                Lastname = "Dash",
                Age = 24,
                Gender = "Female",
                Address = "Patia",
                Phone = "+10000000002",
                Email = "shopkeeper@ims.local",
                Role = RoleName.Staff,
                CreatedAt = DateTime.UtcNow
            };
            secUser.PasswordHash = passwordHasher.HashPassword(secUser, "Manager@123");


            await dbContext.Users.AddRangeAsync(firstUser, secUser);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Suppliers.AnyAsync())
        {
            var suppliers = new[]
            {
                new Supplier { SupplierName = "Northwind Supplies", ContactPerson = "Alice Carter", Phone = "+12025550101", Email = "sales@northwind.com", Address = "New York" },
                new Supplier { SupplierName = "Contoso Wholesale", ContactPerson = "Bob Martin", Phone = "+12025550102", Email = "contact@contoso.com", Address = "Chicago" }
            };

            await dbContext.Suppliers.AddRangeAsync(suppliers);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Products.AnyAsync())
        {
            var supplierId1 = await dbContext.Suppliers.Select(supplier => supplier.SupplierId).OrderBy(id => id).FirstAsync();
            var supplierId2 = await dbContext.Suppliers.Select(supplier => supplier.SupplierId).OrderByDescending(id => id).FirstAsync();

            var products = new[]
            {
                new Product { ProductName = "Laptop", ProductDesc = "Business laptop", ProductPrice = 850.00m, QuantityInStock = 20, SupplierId = supplierId1, CreatedAt = DateTime.UtcNow },
                new Product { ProductName = "Mouse", ProductDesc = "Wireless mouse", ProductPrice = 25.00m, QuantityInStock = 150, SupplierId = supplierId1, CreatedAt = DateTime.UtcNow },
                new Product { ProductName = "Keyboard", ProductDesc = "Mechanical keyboard", ProductPrice = 75.00m, QuantityInStock = 80, SupplierId = supplierId2, CreatedAt = DateTime.UtcNow }
            };

            await dbContext.Products.AddRangeAsync(products);
            await dbContext.SaveChangesAsync();
        }
    }
}
