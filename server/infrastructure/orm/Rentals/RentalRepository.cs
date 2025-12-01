using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Rentals;

public class RentalRepository(AppDbContext context)
    : BaseRepository<Rental>(context), IRepositoryRental
{
    public override async Task<List<Rental>> GetAllAsync() =>
        await WithIncludes().ToListAsync();

    public override async Task<List<Rental>> GetAllAsync(int quantity) =>
        await WithIncludes().Take(quantity).ToListAsync();

    public override async Task<Rental?> GetByIdAsync(Guid entityId) =>
        await WithIncludes().FirstOrDefaultAsync(r => r.Id == entityId);

    private IQueryable<Rental> WithIncludes() =>
        this.records
            .Include(r => r.User)
            .Include(r => r.Employee)
            .Include(r => r.Client)
            .Include(r => r.Driver)
            .Include(r => r.Vehicle)
            .Include(r => r.PricingPlan)
            .Include(r => r.RateServices);
}