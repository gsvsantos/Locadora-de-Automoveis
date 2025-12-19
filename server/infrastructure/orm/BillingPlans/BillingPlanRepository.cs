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

    public async Task<BillingPlan?> GetByGroupId(Guid id)
    {
        return await this.records
            .Include(pp => pp.User)
            .Include(pp => pp.Group)
            .FirstOrDefaultAsync(pp => pp.GroupId.Equals(id));
    }

    public override async Task<List<BillingPlan>> GetAllAsync()
    {
        return await this.records
            .Include(pp => pp.User)
            .Include(pp => pp.Group)
            .ToListAsync();
    }

    public override async Task<List<BillingPlan>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(pp => pp.User)
            .Include(pp => pp.Group)
            .Take(quantity).ToListAsync();
    }

    public override async Task<List<BillingPlan>> GetAllAsync(bool isActive)
    {
        return await this.records
            .Include(pp => pp.User)
            .Include(pp => pp.Group)
            .Where(pp => pp.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<List<BillingPlan>> GetAllAsync(int quantity, bool isActive)
    {
        return await this.records
            .Include(pp => pp.User)
            .Include(pp => pp.Group)
            .Where(pp => pp.IsActive == isActive)
            .Take(quantity)
            .ToListAsync();
    }

    public override async Task<BillingPlan?> GetByIdAsync(Guid id)
    {
        return await this.records
            .Include(pp => pp.User)
            .Include(pp => pp.Group)
            .FirstOrDefaultAsync(pp => pp.Id.Equals(id));
    }
}
