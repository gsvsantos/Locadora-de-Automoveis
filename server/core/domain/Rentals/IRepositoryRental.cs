using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public interface IRepositoryRental : IRepository<Rental>
{
    Task<bool> HasActiveRentalsByEmployee(Guid employeeId);

    Task<bool> HasRentalHistoryByEmployee(Guid employeeId);

    Task<bool> HasActiveRentalsByBillingPlan(Guid BillingPlanId);

    Task<bool> HasRentalHistoryByBillingPlan(Guid BillingPlanId);

    Task<bool> HasActiveRentalsByVehicle(Guid vehicleId);

    Task<bool> HasRentalHistoryByVehicle(Guid vehicleId);

    Task<bool> HasActiveRentalsByClient(Guid clientId);

    Task<bool> HasRentalHistoryByClient(Guid clientId);

    Task<bool> HasActiveRentalsByDriver(Guid driverId);

    Task<bool> HasRentalHistoryByDriver(Guid driverId);

    Task<bool> HasActiveRentalsByRateService(Guid serviceId);

    Task<bool> HasRentalHistoryByRateService(Guid serviceId);

    Task<bool> HasActiveRentalsByCoupon(Guid couponId);

    Task<bool> HasRentalHistoryByCoupon(Guid couponId);

    Task<bool> HasClientUsedCoupon(Guid clientId, Guid couponId);
}