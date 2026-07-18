using InventoryMS.Helpers;
using InventoryMS.Data.Models;
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

        // Ensure roles exist
        if (!await dbContext.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role { Name = RoleName.Owner },
                new Role { Name = RoleName.HR },
                new Role { Name = RoleName.AdminStaff },
                new Role { Name = RoleName.ViewerStaff },
                new Role { Name = RoleName.EditorStaff }
            };

            await dbContext.Roles.AddRangeAsync(roles);
            await dbContext.SaveChangesAsync();
        }

        // Seed one user per role
        if (!await dbContext.Users.AnyAsync())
        {
            var ownerRole = await dbContext.Roles.SingleAsync(r => r.Name == RoleName.Owner);
            var hrRole = await dbContext.Roles.SingleAsync(r => r.Name == RoleName.HR);
            var adminStaffRole = await dbContext.Roles.SingleAsync(r => r.Name == RoleName.AdminStaff);
            var viewerStaffRole = await dbContext.Roles.SingleAsync(r => r.Name == RoleName.ViewerStaff);
            var editorStaffRole = await dbContext.Roles.SingleAsync(r => r.Name == RoleName.EditorStaff);

            // Owner — godamm.warehouse@gmail.com / GoDamm@1097
            var owner = new User
            {
                Firstname = "GoDamm",
                Lastname = "Warehouse",
                Age = 30,
                Gender = "Male",
                Address = "Headquarters",
                Phone = "+10000000001",
                Email = "godamm.warehouse@gmail.com",
                RoleId = ownerRole.RoleId,
                CreatedAt = DateTime.UtcNow
            };
            owner.PasswordHash = passwordHasher.HashPassword(owner, "GoDamm@1097");

            // HR — hr@ims.local / HR@12345
            var hrUser = new User
            {
                Firstname = "Surabhi",
                Lastname = "Dash",
                Age = 24,
                Gender = "Female",
                Address = "Patia",
                Phone = "+10000000002",
                Email = "hr@ims.local",
                RoleId = hrRole.RoleId,
                CreatedAt = DateTime.UtcNow
            };
            hrUser.PasswordHash = passwordHasher.HashPassword(hrUser, "HR@12345");

            // AdminStaff — adminstaff@ims.local / AdminStaff@123
            var adminStaff = new User
            {
                Firstname = "Raj",
                Lastname = "Kumar",
                Age = 28,
                Gender = "Male",
                Address = "Bhubaneswar",
                Phone = "+10000000003",
                Email = "adminstaff@ims.local",
                RoleId = adminStaffRole.RoleId,
                CreatedAt = DateTime.UtcNow
            };
            adminStaff.PasswordHash = passwordHasher.HashPassword(adminStaff, "AdminStaff@123");

            // ViewerStaff — viewerstaff@ims.local / ViewerStaff@123
            var viewerStaff = new User
            {
                Firstname = "Priya",
                Lastname = "Patel",
                Age = 25,
                Gender = "Female",
                Address = "Cuttack",
                Phone = "+10000000004",
                Email = "viewerstaff@ims.local",
                RoleId = viewerStaffRole.RoleId,
                CreatedAt = DateTime.UtcNow
            };
            viewerStaff.PasswordHash = passwordHasher.HashPassword(viewerStaff, "ViewerStaff@123");

            // EditorStaff — editorstaff@ims.local / EditorStaff@123
            var editorStaff = new User
            {
                Firstname = "Ankit",
                Lastname = "Singh",
                Age = 27,
                Gender = "Male",
                Address = "Puri",
                Phone = "+10000000005",
                Email = "editorstaff@ims.local",
                RoleId = editorStaffRole.RoleId,
                CreatedAt = DateTime.UtcNow
            };
            editorStaff.PasswordHash = passwordHasher.HashPassword(editorStaff, "EditorStaff@123");


            await dbContext.Users.AddRangeAsync(owner, hrUser, adminStaff, viewerStaff, editorStaff);
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
