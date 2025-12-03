using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Coupons;

public class CouponRepository(AppDbContext context)
    : BaseRepository<Coupon>(context), IRepositoryCoupon
{
    public async Task<bool> ExistsByPartnerId(Guid partnerId)
    {
        return await this.records.AnyAsync(c => c.PartnerId.Equals(partnerId));
    }

    public async Task<Coupon?> GetByNameAsync(string name)
    {
        return await this.records
            .Include(x => x.User)
            .Include(c => c.Partner)
            .FirstOrDefaultAsync(c => c.Name.Equals(name));
    }

    public override async Task<List<Coupon>> GetAllAsync()
    {
        return await this.records
            .Include(x => x.User)
            .Include(c => c.Partner)
            .ToListAsync();
    }

    public override async Task<List<Coupon>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(x => x.User)
            .Include(c => c.Partner)
            .Take(quantity)
            .ToListAsync();
    }

    public override async Task<Coupon?> GetByIdAsync(Guid id)
    {
        return await this.records
            .Include(x => x.User)
            .Include(c => c.Partner)
            .FirstOrDefaultAsync(c => c.Id.Equals(id));
    }
}
