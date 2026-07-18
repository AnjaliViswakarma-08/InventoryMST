using InventoryMS.Data.Models;

namespace InventoryMS.Data.Repositories.Interfaces;

public interface IOtpRepository
{
    Task AddAsync(OtpEntry entry, CancellationToken cancellationToken);

    Task<OtpEntry?> GetLatestValidAsync(string email, string purpose, CancellationToken cancellationToken);

    Task InvalidateAllForEmailAsync(string email, string purpose, CancellationToken cancellationToken);
}
