using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Vehicles;

public interface IRepositoryVehicle : IRepository<Vehicle>
{
    Task<bool> ExistsByGroupId(Guid groupId);

    Task<List<Vehicle>> GetByGroupIdAsync(Guid groupId);

    Task<List<Vehicle>> SearchAsync(string term, CancellationToken ct = default);

    Task<PagedResult<Vehicle>> GetAllAvailableAsync(int pageNumber, int pageSize, string? term, Guid? groupId, EFuelType? fuelType, CancellationToken cancellationToken);
}
