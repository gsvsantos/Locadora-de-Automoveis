using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.PricingPlans;

public class PricingPlanRepository(AppDbContext context)
    : BaseRepository<PricingPlan>(context), IRepositoryPricingPlan
{
    public override Task<List<PricingPlan>> GetAllAsync() =>
        this.records.Include(pp => pp.Group).ToListAsync();

    public override Task<List<PricingPlan>> GetAllAsync(int quantity) =>
        this.records.Include(pp => pp.Group).Take(quantity).ToListAsync();

    public override Task<PricingPlan?> GetByIdAsync(Guid id) =>
        this.records.Include(pp => pp.Group).FirstOrDefaultAsync(v => v.Id == id);

    public Task<PricingPlan?> GetByGroupId(Guid id) =>
        this.records.Include(pp => pp.Group).FirstOrDefaultAsync(v => v.GroupId == id);
}
