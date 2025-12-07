using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Clients;

public interface IRepositoryClient : IRepository<Client>
{
    Task<bool> BusinessClientHasIndividuals(Guid id);
    Task<List<Client>> GetIndividualClientsFromBusinessId(Guid id, CancellationToken ct = default);
}
