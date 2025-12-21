using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Rentals;

public class RentalReturnRepository(AppDbContext context)
    : BaseRepository<RentalReturn>(context), IRepositoryRentalReturn
{
    public override async Task<List<RentalReturn>> GetAllAsync()
    {
        return await WithIncludes().ToListAsync();
    }

    public override async Task<List<RentalReturn>> GetAllAsync(int quantity)
    {
        return await WithIncludes().Take(quantity).ToListAsync();
    }

    public override async Task<RentalReturn?> GetByIdAsync(Guid entityId)
    {
        return await WithIncludes().FirstOrDefaultAsync(rr => rr.Id == entityId);
    }

    private IQueryable<RentalReturn> WithIncludes()
    {
        return this.records
            .Include(rr => rr.Rental)
                .ThenInclude(r => r.Client)
            .Include(rr => rr.Rental)
                .ThenInclude(r => r.Driver)
            .Include(rr => rr.Rental)
                .ThenInclude(r => r.Vehicle)
                    .ThenInclude(v => v.Group)
            .Include(rr => rr.Rental)
                .ThenInclude(r => r.BillingPlan)
            .Include(rr => rr.Rental)
                .ThenInclude(r => r.Extras)
            .Include(rr => rr.Rental)
                .ThenInclude(r => r.Employee)
            .Include(rr => rr.User);
    }
}