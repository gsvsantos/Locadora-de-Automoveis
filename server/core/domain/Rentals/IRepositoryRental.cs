using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public interface IRepositoryRental : IRepository<Rental>
{
    Task<bool> HasActiveRentalsByEmployee(Guid employeeId);

    Task<bool> HasRentalHistoryByEmployee(Guid employeeId);

    Task<bool> HasActiveRentalsByBillingPlan(Guid BillingPlanId);

    Task<bool> HasRentalHistoryByBillingPlan(Guid BillingPlanId);

    Task<bool> HasActiveRentalsByVehicleDistinctAsync(Guid vehicleId);

    Task<bool> HasActiveRentalsByVehicle(Guid vehicleId);

    Task<bool> HasRentalHistoryByVehicle(Guid vehicleId);

    Task<bool> HasActiveRentalsByClient(Guid clientId);

    Task<bool> HasRentalHistoryByClient(Guid clientId);

    Task<bool> HasActiveRentalsByDriverDistinctAsync(Guid driverId);

    Task<bool> HasActiveRentalsByDriver(Guid driverId);

    Task<bool> HasRentalHistoryByDriver(Guid driverId);

    Task<bool> HasActiveRentalsByRentalExtra(Guid extraId);

    Task<bool> HasRentalHistoryByRentalExtra(Guid extraId);

    Task<bool> HasCouponUsedByClientDistinctAsync(Guid clientId, Guid couponId);

    Task<bool> HasActiveRentalsByCoupon(Guid couponId);

    Task<bool> HasRentalHistoryByCoupon(Guid couponId);

    Task<bool> HasClientUsedCoupon(Guid clientId, Guid couponId);

    Task<List<Rental>> SearchAsync(string term, CancellationToken ct = default);

    Task<PagedResult<Rental>> GetMyRentalsDistinctAsync(Guid loginUserId, int pageNumber, int pageSize, string? term, Guid? tenantId, ERentalStatus? status, CancellationToken cancellationToken);

    Task<Rental?> GetMyByIdDistinctAsync(Guid rentalId, Guid loginUserId);

    Task<List<Guid>> GetRentedVehicleIds();
}