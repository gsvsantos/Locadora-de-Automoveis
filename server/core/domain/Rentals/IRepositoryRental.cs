using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public interface IRepositoryRental : IRepository<Rental>
{
    Task<bool> HasActiveRentalsByEmployee(Guid employeeId);

    Task<bool> HasRentalHistoryByEmployee(Guid employeeId);

    Task<bool> HasClientUsedCoupon(Guid clientId, Guid couponId);
}