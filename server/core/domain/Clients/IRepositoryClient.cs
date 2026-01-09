using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Clients;

public interface IRepositoryClient : IRepository<Client>
{
    Task<bool> BusinessClientHasIndividuals(Guid id);

    Task<bool> ExistsByDocumentAsync(string document);

    Task<bool> ExistsByDocumentAsync(string document, Guid ignoreClientId);

    Task<bool> UpdateGlobalAsync(Guid clientId, Client updatedClient);

    Task<List<Client>> GetIndividualClientsFromBusinessIdAsync(Guid id, List<Guid> driversIds, CancellationToken ct = default);

    Task<List<Client>> SearchAsync(string term, CancellationToken ct = default);

    Task<Client?> GetByTenantAndLoginUserIdAsync(Guid tenantId, Guid loginUserId);

    Task<Client?> GetGlobalByLoginUserIdAsync(Guid loginUserId);

    Task<Client?> GetByTenantAndDocumentAsync(Guid tenantId, string document);
}
