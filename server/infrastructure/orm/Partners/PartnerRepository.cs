using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Partners;

public class PartnerRepository(AppDbContext context)
    : BaseRepository<Partner>(context), IRepositoryPartner
{
    public override async Task<List<Partner>> GetAllAsync()
    {
        return await this.records
            .Include(p => p.User)
            .Include(p => p.Coupons)
            .ToListAsync();
    }

    public override async Task<List<Partner>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(p => p.User)
            .Include(p => p.Coupons)
            .Take(quantity).ToListAsync();
    }

    public override async Task<Partner?> GetByIdAsync(Guid entityId)
    {
        return await this.records
            .Include(p => p.User)
            .Include(p => p.Coupons)
            .FirstOrDefaultAsync(p => p.Id.Equals(entityId));
    }
}
