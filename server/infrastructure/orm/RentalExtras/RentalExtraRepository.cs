using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.RentalExtras;

public class RentalExtraRepository(AppDbContext context)
    : BaseRepository<RentalExtra>(context), IRepositoryRentalExtra
{
    public async Task<List<RentalExtra>> GetManyByIds(List<Guid> ids)
    {
        return await this.records
            .Where(re => ids.Contains(re.Id))
            .Where(re => re.IsActive == true)
            .ToListAsync();
    }

    public async Task<List<RentalExtra>> GetManyByTenantAndIdsDistinctAsync(Guid tenantId, List<Guid> ids)
    {
        return await this.records
            .IgnoreQueryFilters()
            .Where(re => ids.Contains(re.Id))
            .Where(re => re.TenantId.Equals(tenantId))
            .Where(re => re.IsActive == true)
            .ToListAsync();
    }

    public async Task<List<RentalExtra>> GetAllByTenantDistinctAsync(Guid tenantId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .Where(d => d.TenantId.Equals(tenantId))
            .Where(d => d.IsActive == true)
            .ToListAsync();
    }
}
