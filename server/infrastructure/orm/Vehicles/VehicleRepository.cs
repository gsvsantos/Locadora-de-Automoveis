using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Vehicles;

public class VehicleRepository(AppDbContext context)
    : BaseRepository<Vehicle>(context), IRepositoryVehicle
{
    public async Task<bool> ExistsByGroupId(Guid groupId)
    {
        return await this.records
            .AnyAsync(x => x.GroupId.Equals(groupId));
    }

    public async Task<List<Vehicle>> GetByGroupIdAsync(Guid groupId)
    {
        return await this.records
            .Include(v => v.User)
            .Include(v => v.Group)
            .Where(v => v.GroupId.Equals(groupId))
            .ToListAsync();
    }

    public override async Task<List<Vehicle>> GetAllAsync()
    {
        return await this.records
            .Include(v => v.User)
            .Include(v => v.Group)
            .ToListAsync();
    }

    public override async Task<List<Vehicle>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(v => v.User)
            .Include(v => v.Group)
            .Take(quantity).ToListAsync();
    }

    public override async Task<List<Vehicle>> GetAllAsync(bool isActive)
    {
        return await this.records
            .Include(v => v.User)
            .Include(v => v.Group)
            .Where(v => v.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<List<Vehicle>> GetAllAsync(int quantity, bool isActive)
    {
        return await this.records
            .Include(v => v.User)
            .Include(v => v.Group)
            .Take(quantity)
            .Where(v => v.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<Vehicle?> GetByIdAsync(Guid entityId)
    {
        return await this.records
            .Include(v => v.User)
            .Include(v => v.Group)
            .FirstOrDefaultAsync(v => v.Id.Equals(entityId));
    }
}
