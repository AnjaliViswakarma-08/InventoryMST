using InventoryMS.Data.Repositories.Interfaces;
using InventoryMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Users.AsNoTracking().Include(u => u.Role).OrderBy(u => u.UserId).ToListAsync(cancellationToken);

    public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken) =>
        _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);

    public Task<bool> EmailExistsAsync(string email, int? excludeUserId, CancellationToken cancellationToken) =>
        _dbContext.Users.AnyAsync(u =>
            u.Email.ToLower() == email.ToLower() && (!excludeUserId.HasValue || u.UserId != excludeUserId.Value),
            cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken) =>
        _dbContext.Users.AddAsync(user, cancellationToken).AsTask();

    public void Update(User user) => _dbContext.Users.Update(user);

    public void Remove(User user) => _dbContext.Users.Remove(user);

    public Task LoadRoleAsync(User user, CancellationToken cancellationToken) =>
        _dbContext.Entry(user).Reference(u => u.Role).LoadAsync(cancellationToken);
}
