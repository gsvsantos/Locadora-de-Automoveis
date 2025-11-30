using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Configurations;

public class ConfigurationRepository(AppDbContext context)
    : BaseRepository<Configuration>(context), IRepositoryConfiguration
{
    public async Task<Configuration?> GetByTenantId(Guid tenantId)
    {
        return await this.records
            .FirstOrDefaultAsync(c => c.TenantId == tenantId);
    }
}
