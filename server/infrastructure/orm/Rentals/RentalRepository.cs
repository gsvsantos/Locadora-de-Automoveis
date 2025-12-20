using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Rentals;

public class RentalRepository(AppDbContext context)
    : BaseRepository<Rental>(context), IRepositoryRental
{
    public async Task<bool> HasActiveRentalsByEmployee(Guid employeeId)
    {
        return await this.records
            .AnyAsync(r =>
            r.EmployeeId.Equals(employeeId) &&
            r.Status == ERentalStatus.Open);
    }

    public async Task<bool> HasRentalHistoryByEmployee(Guid employeeId)
    {
        return await this.records
            .AnyAsync(r =>
            r.EmployeeId.Equals(employeeId) &&
            r.Status != ERentalStatus.Open);
    }

    public async Task<bool> HasActiveRentalsByBillingPlan(Guid BillingPlanId)
    {
        return await this.records.AnyAsync(r =>
            r.BillingPlanId.Equals(BillingPlanId) &&
            r.Status == ERentalStatus.Open);
    }

    public async Task<bool> HasRentalHistoryByBillingPlan(Guid BillingPlanId)
    {
        return await this.records.AnyAsync(r =>
            r.BillingPlanId.Equals(BillingPlanId) &&
            r.Status != ERentalStatus.Open);
    }

    public async Task<bool> HasActiveRentalsByVehicleDistinctAsync(Guid vehicleId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .AnyAsync(r =>
                r.VehicleId.Equals(vehicleId) &&
                r.Status == ERentalStatus.Open
            );
    }

    public async Task<bool> HasActiveRentalsByVehicle(Guid vehicleId)
    {
        return await this.records.AnyAsync(r =>
            r.VehicleId.Equals(vehicleId) &&
            r.Status == ERentalStatus.Open);
    }

    public async Task<bool> HasRentalHistoryByVehicle(Guid vehicleId)
    {
        return await this.records.AnyAsync(r =>
            r.VehicleId.Equals(vehicleId) &&
            r.Status != ERentalStatus.Open);
    }

    public async Task<bool> HasActiveRentalsByClient(Guid clientId)
    {
        return await this.records.AnyAsync(r =>
            r.ClientId.Equals(clientId) &&
            r.Status == ERentalStatus.Open);
    }

    public async Task<bool> HasRentalHistoryByClient(Guid clientId)
    {
        return await this.records.AnyAsync(r =>
            r.ClientId.Equals(clientId) &&
            r.Status != ERentalStatus.Open);
    }

    public async Task<bool> HasActiveRentalsByDriverDistinctAsync(Guid driverId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .AnyAsync(r =>
                r.DriverId.Equals(driverId) &&
                r.Status == ERentalStatus.Open
            );
    }

    public async Task<bool> HasActiveRentalsByDriver(Guid driverId)
    {
        return await this.records.AnyAsync(r =>
            r.DriverId.Equals(driverId) &&
            r.Status == ERentalStatus.Open);
    }

    public async Task<bool> HasRentalHistoryByDriver(Guid driverId)
    {
        return await this.records.AnyAsync(r =>
            r.DriverId.Equals(driverId) &&
            r.Status != ERentalStatus.Open);
    }

    public async Task<bool> HasActiveRentalsByRentalExtra(Guid extraId)
    {
        return await this.records.AnyAsync(r =>
            r.Status == ERentalStatus.Open &&
            r.Extras.Any(s => s.Id.Equals(extraId)));
    }

    public async Task<bool> HasRentalHistoryByRentalExtra(Guid extraId)
    {
        return await this.records.AnyAsync(r =>
            r.Extras.Any(s => s.Id.Equals(extraId)));
    }

    public async Task<bool> HasCouponUsedByClientDistinctAsync(Guid clientId, Guid couponId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .AnyAsync(r =>
                r.ClientId.Equals(clientId) &&
                r.CouponId.Equals(couponId) &&
                r.Status != ERentalStatus.Canceled
            );
    }

    public async Task<bool> HasClientUsedCoupon(Guid clientId, Guid couponId)
    {
        return await this.records.AnyAsync(r =>
                r.ClientId.Equals(clientId) &&
                r.CouponId.Equals(couponId) &&
                r.Status != ERentalStatus.Canceled);
    }

    public async Task<bool> HasActiveRentalsByCoupon(Guid couponId)
    {
        return await this.records.AnyAsync(r =>
            r.CouponId == couponId &&
            r.Status == ERentalStatus.Open);
    }

    public async Task<bool> HasRentalHistoryByCoupon(Guid couponId)
    {
        return await this.records.AnyAsync(r => r.CouponId == couponId);
    }

    public async Task<List<Rental>> SearchAsync(string term, CancellationToken ct)
    {
        return await this.records
            .AsNoTracking()
            .Include(r => r.Client)
            .Include(r => r.Vehicle)
            .Include(r => r.Driver)
            .Where(r =>
                r.Client.FullName.ToLower().Contains(term) ||
                r.Driver.FullName.ToLower().Contains(term) ||
                r.Vehicle.Model.ToLower().Contains(term) ||
                r.Vehicle.LicensePlate.ToLower().Contains(term)
            )
            .OrderByDescending(r => r.StartDate)
            .Take(5)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<Rental>> GetMyRentalsDistinctAsync(
        Guid loginUserId, int pageNumber,
        int pageSize, string? term,
        Guid? tenantId, ERentalStatus? status,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Rental> query = this.records
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(r => r.Client)
            .Include(r => r.Vehicle)
            .Include(r => r.Driver)
            .Where(r => r.IsActive)
            .Where(r => r.Client.LoginUserId.Equals(loginUserId));

        if (!string.IsNullOrWhiteSpace(term))
        {
            string termLower = term.Trim();
            query = query.Where(r =>
                r.Vehicle.Model.ToLower().Contains(termLower) ||
                r.Vehicle.LicensePlate.ToLower().Contains(termLower));
        }

        if (tenantId.HasValue && tenantId.Value != Guid.Empty)
        {
            query = query.Where(r => r.TenantId.Equals(tenantId.Value));
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status.Equals(status.Value));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Rental> rentals = await query
            .OrderByDescending(r => r.StartDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Rental>(rentals, totalCount, pageNumber, pageSize);
    }

    public async Task<Rental?> GetMyByIdDistinctAsync(Guid rentalId, Guid loginUserId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(r => r.RentalReturn)
            .Include(r => r.Client)
            .Include(r => r.Driver)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v.Group)
            .Include(r => r.Coupon)
                .ThenInclude(c => c!.Partner)
            .Include(r => r.BillingPlan)
            .Include(r => r.Extras)
            .FirstOrDefaultAsync(r =>
                r.Id.Equals(rentalId) &&
                r.Client.LoginUserId.Equals(loginUserId)
            );
    }

    public async Task<List<Guid>> GetRentedVehicleIds()
    {
        return await this.records
            .IgnoreQueryFilters()
            .Where(r => r.Status == ERentalStatus.Open)
            .Select(r => r.VehicleId)
            .ToListAsync();
    }

    public override async Task<List<Rental>> GetAllAsync()
    {
        return await WithIncludes().ToListAsync();
    }

    public override async Task<List<Rental>> GetAllAsync(int quantity)
    {
        return await WithIncludes().Take(quantity).ToListAsync();
    }

    public override async Task<List<Rental>> GetAllAsync(bool isActive)
    {
        return await WithIncludes()
            .Where(d => d.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<List<Rental>> GetAllAsync(int quantity, bool isActive)
    {
        return await WithIncludes()
            .Take(quantity)
            .Where(d => d.IsActive == isActive)
            .ToListAsync();
    }

    public override async Task<Rental?> GetByIdAsync(Guid entityId)
    {
        return await WithIncludes().FirstOrDefaultAsync(r => r.Id.Equals(entityId));
    }

    private IQueryable<Rental> WithIncludes()
    {
        return this.records
            .Include(r => r.RentalReturn)
            .Include(r => r.Employee)
            .Include(r => r.Client)
            .Include(r => r.Driver)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v.Group)
            .Include(r => r.Coupon)
                .ThenInclude(c => c!.Partner)
            .Include(r => r.BillingPlan)
            .Include(r => r.Extras);
    }
}