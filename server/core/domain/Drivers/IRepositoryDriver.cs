using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Drivers;

public interface IRepositoryDriver : IRepository<Driver>
{
    Task<bool> HasDriversByClient(Guid clientId);

    Task<List<Guid?>> GetTenantsWithActiveDriversAsync();

    Task<List<Guid>> GetDriverClientIdsAsync();

    Task<Driver?> GetByDocumentAsync(string document);

    Task<Driver?> GetDriverByClientId(Guid clientId);

    Task<List<Driver>> SearchAsync(string term, CancellationToken ct = default);

    Task<Driver?> GetByTenantAndIdAsync(Guid tenantId, Guid entityId);

    Task<List<Driver>> GetAllByTenantDistinctAsync(Guid tenantId);

    Task<Driver?> GetDriverByClientIdDistinctAsync(Guid clientId);
}
