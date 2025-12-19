using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Groups;

public class GroupRepository(AppDbContext context)
    : BaseRepository<Group>(context), IRepositoryGroup
{
    public async Task<List<Group>> SearchAsync(string term, CancellationToken ct)
    {
        return await this.records
            .AsNoTracking()
            .Where(g =>
                g.Name.ToLower().Contains(term)
            )
            .Take(5)
            .ToListAsync(ct);
    }

    public async Task<List<Group>> GetAllDistinct()
    {
        return await this.records
            .IgnoreQueryFilters()
            .Include(g => g.User)
            .Include(g => g.Vehicles)
            .Where(g => g.IsActive == true)
            .ToListAsync();
    }

    public override async Task<List<Group>> GetAllAsync()
    {
        return await this.records
            .Include(g => g.User)
            .Include(g => g.Vehicles)
            .ToListAsync();
    }

    public override async Task<List<Group>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(g => g.User)
            .Include(g => g.Vehicles)
            .Take(quantity).ToListAsync();
    }

    public override async Task<List<Group>> GetAllAsync(bool isActive)
    {
        return await this.records
            .Include(g => g.User)
            .Include(g => g.Vehicles)
            .Where(g => g.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<List<Group>> GetAllAsync(int quantity, bool isActive)
    {
        return await this.records
            .Include(g => g.User)
            .Include(g => g.Vehicles)
            .Where(g => g.IsActive == isActive)
            .Take(quantity)
            .ToListAsync();
    }

    public override async Task<Group?> GetByIdAsync(Guid entityId)
    {
        return await this.records
            .Include(g => g.User)
            .Include(g => g.Vehicles)
            .FirstOrDefaultAsync(g => g.Id.Equals(entityId));
    }
}
