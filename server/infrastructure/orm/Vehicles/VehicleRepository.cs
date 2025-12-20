using LocadoraDeAutomoveis.Domain.Shared;
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
            .Include(v => v.Group)
            .Where(v => v.GroupId.Equals(groupId))
            .Where(v => v.IsActive == true)
            .ToListAsync();
    }

    public async Task<List<Vehicle>> SearchAsync(string term, CancellationToken ct)
    {
        return await this.records
            .AsNoTracking()
            .Where(v =>
                v.Model.ToLower().Contains(term) ||
                v.Brand.ToLower().Contains(term) ||
                v.LicensePlate.ToLower().Contains(term))
            .Take(5)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<Vehicle>> GetAllAvailableAsync(
        int pageNumber, int pageSize,
        string? term, Guid? groupId,
        EFuelType? fuelType, CancellationToken cancellationToken
    )
    {
        IQueryable<Vehicle> query = this.records
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(v => v.Group)
            .Where(v => v.IsActive == true);

        if (!string.IsNullOrWhiteSpace(term))
        {
            string termLower = term.ToLower();
            query = query.Where(v =>
                v.Model.ToLower().Contains(termLower) ||
                v.Brand.ToLower().Contains(termLower) ||
                v.LicensePlate.ToLower().Contains(termLower));
        }

        if (groupId.HasValue)
        {
            query = query.Where(v => v.GroupId.Equals(groupId.Value));
        }

        if (fuelType.HasValue)
        {
            query = query.Where(v => v.FuelType.Equals(fuelType.Value));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Vehicle> items = await query
            .OrderBy(v => v.Group.Name)
            .ThenBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Vehicle?> GetByIdDistinctAsync(Guid id)
    {
        return await this.records
            .IgnoreQueryFilters()
            .Include(v => v.Group)
            .Where(v => v.IsActive == true)
            .FirstOrDefaultAsync(v => v.Id.Equals(id));
    }

    public override async Task<List<Vehicle>> GetAllAsync()
    {
        return await this.records
            .Include(v => v.Group)
            .ToListAsync();
    }

    public override async Task<List<Vehicle>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(v => v.Group)
            .Take(quantity).ToListAsync();
    }

    public override async Task<List<Vehicle>> GetAllAsync(bool isActive)
    {
        return await this.records
            .Include(v => v.Group)
            .Where(v => v.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<List<Vehicle>> GetAllAsync(int quantity, bool isActive)
    {
        return await this.records
            .Include(v => v.Group)
            .Take(quantity)
            .Where(v => v.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<Vehicle?> GetByIdAsync(Guid entityId)
    {
        return await this.records
            .Include(v => v.Group)
            .FirstOrDefaultAsync(v => v.Id.Equals(entityId));
    }
}
