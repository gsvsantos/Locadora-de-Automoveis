using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Rentals;

public class RentalRepository(AppDbContext context)
    : BaseRepository<Rental>(context), IRepositoryRental
{
    public override async Task<List<Rental>> GetAllAsync()
    {
        return await this.records
            .Include(r => r.Employee)
            .Include(r => r.Client)
            .Include(r => r.Driver)
            .Include(r => r.Vehicle)
            .Include(r => r.PricingPlan)
            .Include(r => r.RateServices)
            .ToListAsync();
    }

    public override async Task<List<Rental>> GetAllAsync(int quantity)
    {
        return await this.records
            .Include(r => r.Employee)
            .Include(r => r.Client)
            .Include(r => r.Driver)
            .Include(r => r.Vehicle)
            .Include(r => r.PricingPlan)
            .Include(r => r.RateServices)
            .Take(quantity).ToListAsync();
    }

    public override async Task<Rental?> GetByIdAsync(Guid entityId)
    {
        return await this.records
            .Include(r => r.Employee)
            .Include(r => r.Client)
            .Include(r => r.Driver)
            .Include(r => r.Vehicle)
            .Include(r => r.PricingPlan)
            .Include(r => r.RateServices)
            .FirstOrDefaultAsync(d => d.Id.Equals(entityId));
    }
}