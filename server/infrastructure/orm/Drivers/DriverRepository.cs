using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Drivers;

public class DriverRepository(AppDbContext context)
    : BaseRepository<Driver>(context), IRepositoryDriver
{
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

    public override async Task<Driver?> GetByIdAsync(Guid entityId)
    {
        return await this.records
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id.Equals(entityId));
    }
}
