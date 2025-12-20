using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Drivers;

public class DriverRepository(AppDbContext context)
    : BaseRepository<Driver>(context), IRepositoryDriver
{
    public async Task<bool> HasDriversByClient(Guid clientId)
    {
        return await this.records.AnyAsync(r =>
            r.ClientId.Equals(clientId));
    }

    public async Task<Driver?> GetDriverByClientId(Guid clientId)
    {
        return await this.records
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Client.Id.Equals(clientId));
    }

    public async Task<List<Driver>> SearchAsync(string term, CancellationToken ct)
    {
        return await this.records
            .AsNoTracking()
            .Where(d =>
                d.FullName.ToLower().Contains(term) ||
                d.Document.ToLower().Contains(term)
            )
            .Take(5)
            .ToListAsync(ct);
    }

    public async Task<Driver?> GetByTenantAndIdAsync(Guid tenantId, Guid entityId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .Include(d => d.Client)
            .Where(d => d.TenantId.Equals(tenantId))
            .FirstOrDefaultAsync(d => d.Id.Equals(entityId));
    }

    public async Task<List<Driver>> GetAllByTenantDistinctAsync(Guid tenantId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .Include(d => d.Client)
            .Where(d => d.TenantId.Equals(tenantId))
            .ToListAsync();
    }

    public override async Task<List<Driver>> GetAllAsync()
    {
        return await this.records
            .Include(d => d.Client)
            .ToListAsync();
    }

    public override async Task<List<Driver>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(d => d.Client)
            .Take(quantity).ToListAsync();
    }

    public override async Task<List<Driver>> GetAllAsync(bool isActive)
    {
        return await this.records
            .Include(d => d.Client)
            .Where(d => d.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<List<Driver>> GetAllAsync(int quantity, bool isActive)
    {
        return await this.records
            .Include(d => d.Client)
            .Where(d => d.IsActive == isActive)
            .Take(quantity)
            .ToListAsync();
    }

    public override async Task<Driver?> GetByIdAsync(Guid entityId)
    {
        return await this.records
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id.Equals(entityId));
    }
}
