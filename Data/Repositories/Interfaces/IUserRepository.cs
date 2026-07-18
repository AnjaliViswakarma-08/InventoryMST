using InventoryMS.Data.Models;

namespace InventoryMS.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, int? excludeUserId, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);

    void Update(User user);

    void Remove(User user);

    Task LoadRoleAsync(User user, CancellationToken cancellationToken);
}
