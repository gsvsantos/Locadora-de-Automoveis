using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.RateServices;

public class RateServiceRepository(AppDbContext context)
    : BaseRepository<RateService>(context), IRepositoryRateService
{
    public Task<List<RateService>> GetMultiplyByIds(List<Guid> ids) =>
        this.records.Include(x => x.User)
        .Where(rs => ids.Contains(rs.Id)).ToListAsync();
}
