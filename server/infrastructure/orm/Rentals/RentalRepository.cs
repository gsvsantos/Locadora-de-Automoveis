using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Rentals;

public class RentalRepository(AppDbContext context)
    : BaseRepository<Rental>(context), IRepositoryRental
{
    public async Task<bool> HasClientUsedCoupon(Guid clientId, Guid couponId)
    {
        return await this.records
            .AnyAsync(r =>
                r.ClientId.Equals(clientId) &&
                r.CouponId.Equals(couponId) &&
                r.Status != ERentalStatus.Canceled
            );
    }

    public override async Task<List<Rental>> GetAllAsync()
    {
        return await WithIncludes().ToListAsync();
    }

    public override async Task<List<Rental>> GetAllAsync(int quantity)
    {
        return await WithIncludes().Take(quantity).ToListAsync();
    }

    public override async Task<Rental?> GetByIdAsync(Guid entityId)
    {
        return await WithIncludes().FirstOrDefaultAsync(r => r.Id == entityId);
    }

    private IQueryable<Rental> WithIncludes()
    {
        return this.records
            .Include(r => r.User)
            .Include(r => r.Employee)
            .Include(r => r.Client)
            .Include(r => r.Driver)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v.Group)
            .Include(r => r.Coupon)
                .ThenInclude(c => c!.Partner)
            .Include(r => r.PricingPlan)
            .Include(r => r.RateServices);
    }
}