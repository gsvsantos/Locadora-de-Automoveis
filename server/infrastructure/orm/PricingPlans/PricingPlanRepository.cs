using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.PricingPlans;

public class PricingPlanRepository(AppDbContext context)
    : BaseRepository<PricingPlan>(context), IRepositoryPricingPlan
{
    public async Task<bool> ExistsByGroupId(Guid groupId)
    {
        return await this.records
            .AnyAsync(x => x.GroupId.Equals(groupId));
    }

    public override async Task<List<PricingPlan>> GetAllAsync()
    {
        return await this.records
            .Include(pp => pp.Group)
            .ToListAsync();
    }

    public override async Task<List<PricingPlan>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(pp => pp.Group)
            .Take(quantity).ToListAsync();
    }

    public override async Task<PricingPlan?> GetByIdAsync(Guid id)
    {
        return await this.records
            .Include(pp => pp.Group)
            .FirstOrDefaultAsync(v => v.Id.Equals(id));
    }

    public async Task<PricingPlan?> GetByGroupId(Guid id)
    {
        return await this.records
            .Include(pp => pp.Group)
            .FirstOrDefaultAsync(v => v.GroupId.Equals(id));
    }
}
