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
            .Include(c => c.User)
            .Include(c => c.Partner)
            .FirstOrDefaultAsync(c => c.Name.Equals(name));
    }

    public async Task<List<Coupon>> SearchAsync(string term, CancellationToken ct)
    {
        return await this.records
            .AsNoTracking()
            .Include(c => c.Partner)
            .Where(c =>
                c.Name.ToLower().Contains(term) ||
                c.Partner.FullName.ToLower().Contains(term)
            )
            .Take(5)
            .ToListAsync(ct);
    }

    public async Task<List<Coupon>> GetAllAvailableAsync()
    {
        return await this.records
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Partner)
            .Where(c => c.ExpirationDate >= DateTimeOffset.UtcNow)
            .Where(c => c.IsManuallyDisabled == false)
            .Where(c => c.IsActive == true)
            .ToListAsync();
    }

    public override async Task<List<Coupon>> GetAllAsync()
    {
        return await this.records
            .Include(c => c.User)
            .Include(c => c.Partner)
            .ToListAsync();
    }

    public override async Task<List<Coupon>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(c => c.User)
            .Include(c => c.Partner)
            .Take(quantity)
            .ToListAsync();
    }

    public override async Task<List<Coupon>> GetAllAsync(bool isActive)
    {
        return await this.records
            .Include(c => c.User)
            .Include(c => c.Partner)
            .Where(c => c.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<List<Coupon>> GetAllAsync(int quantity, bool isActive)
    {
        return await this.records
            .Include(c => c.User)
            .Include(c => c.Partner)
            .Where(c => c.IsActive == isActive)
            .Take(quantity)
            .ToListAsync();
    }

    public override async Task<Coupon?> GetByIdAsync(Guid id)
    {
        return await this.records
            .Include(c => c.User)
            .Include(c => c.Partner)
            .FirstOrDefaultAsync(c => c.Id.Equals(id));
    }
}
