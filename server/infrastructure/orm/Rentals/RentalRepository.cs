using LocadoraDeAutomoveis.Domain.Rentals;
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

    public async Task<bool> HasActiveRentalsByPricingPlan(Guid pricingPlanId)
    {
        return await this.records.AnyAsync(r =>
            r.PricingPlanId.Equals(pricingPlanId) &&
            r.Status == ERentalStatus.Open);
    }

    public async Task<bool> HasRentalHistoryByPricingPlan(Guid pricingPlanId)
    {
        return await this.records.AnyAsync(r =>
            r.PricingPlanId.Equals(pricingPlanId) &&
            r.Status != ERentalStatus.Open);
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

    public async Task<bool> HasActiveRentalsByRateService(Guid serviceId)
    {
        return await this.records.AnyAsync(r =>
            r.Status == ERentalStatus.Open &&
            r.RateServices.Any(s => s.Id.Equals(serviceId)));
    }

    public async Task<bool> HasRentalHistoryByRateService(Guid serviceId)
    {
        return await this.records.AnyAsync(r =>
            r.RateServices.Any(s => s.Id.Equals(serviceId)));
    }

    public async Task<bool> HasClientUsedCoupon(Guid clientId, Guid couponId)
    {
        return await this.records
            .AnyAsync(r =>
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