using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public interface IRepositoryRental : IRepository<Rental>
{
    Task<bool> HasActiveRentalsByEmployee(Guid employeeId);

    Task<bool> HasRentalHistoryByEmployee(Guid employeeId);

    Task<bool> HasActiveRentalsByPricingPlan(Guid pricingPlanId);

    Task<bool> HasRentalHistoryByPricingPlan(Guid pricingPlanId);

    Task<bool> HasClientUsedCoupon(Guid clientId, Guid couponId);
}