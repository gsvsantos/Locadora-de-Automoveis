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
            .Include(re => re.User)
            .Where(re => ids.Contains(re.Id))
            .ToListAsync();
    }
}
