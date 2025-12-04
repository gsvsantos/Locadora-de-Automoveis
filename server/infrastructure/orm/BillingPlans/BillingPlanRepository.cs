using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.BillingPlans;

public class BillingPlanRepository(AppDbContext context)
    : BaseRepository<BillingPlan>(context), IRepositoryBillingPlan
{
    public async Task<bool> ExistsByGroupId(Guid groupId)
    {
        return await this.records
            .AnyAsync(x => x.GroupId.Equals(groupId));
    }

    public override async Task<List<BillingPlan>> GetAllAsync()
    {
        return await this.records
            .Include(pp => pp.Group)
            .ToListAsync();
    }

    public override async Task<List<BillingPlan>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(pp => pp.Group)
            .Take(quantity).ToListAsync();
    }

    public override async Task<BillingPlan?> GetByIdAsync(Guid id)
    {
        return await this.records
            .Include(pp => pp.Group)
            .FirstOrDefaultAsync(v => v.Id.Equals(id));
    }

    public async Task<BillingPlan?> GetByGroupId(Guid id)
    {
        return await this.records
            .Include(pp => pp.Group)
            .FirstOrDefaultAsync(v => v.GroupId.Equals(id));
    }
}
