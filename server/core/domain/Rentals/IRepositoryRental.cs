using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public interface IRepositoryRental : IRepository<Rental>
{
    Task<bool> HasActiveRentalsByEmployee(Guid employeeId);

    Task<bool> HasRentalHistoryByEmployee(Guid employeeId);

    Task<bool> HasActiveRentalsByPricingPlan(Guid pricingPlanId);

    Task<bool> HasRentalHistoryByPricingPlan(Guid pricingPlanId);

    Task<bool> HasActiveRentalsByVehicle(Guid vehicleId);

    Task<bool> HasRentalHistoryByVehicle(Guid vehicleId);

    Task<bool> HasActiveRentalsByClient(Guid clientId);

    Task<bool> HasRentalHistoryByClient(Guid clientId);

    Task<bool> HasClientUsedCoupon(Guid clientId, Guid couponId);
}