using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Clients;

public interface IRepositoryClient : IRepository<Client>
{
    Task<bool> BusinessClientHasIndividuals(Guid id);

    Task<List<Client>> GetIndividualClientsFromBusinessId(Guid id, CancellationToken ct = default);

    Task<List<Client>> SearchAsync(string term, CancellationToken ct = default);
}
