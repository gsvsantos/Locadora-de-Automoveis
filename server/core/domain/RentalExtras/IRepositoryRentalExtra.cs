using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.RentalExtras;

public interface IRepositoryRentalExtra : IRepository<RentalExtra>
{
    Task<List<RentalExtra>> GetManyByIds(List<Guid> ids);

    Task<List<RentalExtra>> GetManyByTenantAndIdsDistinctAsync(Guid tenantId, List<Guid> ids);

    Task<List<RentalExtra>> GetAllByTenantDistinctAsync(Guid tenantId);
}