using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Vehicles;

public interface IRepositoryVehicle : IRepository<Vehicle>
{
    Task<bool> ExistsByGroupId(Guid groupId);

    Task<List<Vehicle>> GetByGroupIdAsync(Guid groupId);

    Task<List<Vehicle>> SearchAsync(string term, CancellationToken ct = default);
}
