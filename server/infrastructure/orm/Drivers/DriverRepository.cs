using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Drivers;

public class DriverRepository(AppDbContext context)
    : BaseRepository<Driver>(context), IRepositoryDriver
{
    public override Task<List<Driver>> GetAllAsync() =>
        this.records.Include(d => d.Client)
        .Include(d => d.ClientCNPJ)
        .ToListAsync();

    public override Task<List<Driver>> GetAllAsync(int quantity) =>
        this.records.Include(d => d.Client)
        .Include(d => d.ClientCNPJ)
        .Take(quantity).ToListAsync();

    public override Task<Driver?> GetByIdAsync(Guid entityId) =>
        this.records.Include(d => d.Client)
        .Include(d => d.ClientCNPJ)
        .FirstOrDefaultAsync(d => d.Id.Equals(entityId));
}
