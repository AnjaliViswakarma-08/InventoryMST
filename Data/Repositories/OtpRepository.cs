using InventoryMS.Data.Models;
using InventoryMS.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Data.Repositories;

public sealed class OtpRepository : IOtpRepository
{
    private readonly AppDbContext _dbContext;

    public OtpRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(OtpEntry entry, CancellationToken cancellationToken) =>
        _dbContext.OtpEntries.AddAsync(entry, cancellationToken).AsTask();

    public Task<OtpEntry?> GetLatestValidAsync(string email, string purpose, CancellationToken cancellationToken) =>
        _dbContext.OtpEntries
            .Where(e => e.Email.ToLower() == email.ToLower()
                && e.Purpose == purpose
                && !e.IsUsed
                && e.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(e => e.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task InvalidateAllForEmailAsync(string email, string purpose, CancellationToken cancellationToken)
    {
        var entries = await _dbContext.OtpEntries
            .Where(e => e.Email.ToLower() == email.ToLower()
                && e.Purpose == purpose
                && !e.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var entry in entries)
        {
            entry.IsUsed = true;
        }
    }
}
