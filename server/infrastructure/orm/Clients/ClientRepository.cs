using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Clients;

public class ClientRepository(AppDbContext context)
    : BaseRepository<Client>(context), IRepositoryClient
{
    public async Task<bool> BusinessClientHasIndividuals(Guid id)
    {
        return await this.records
            .AnyAsync(x => x.JuristicClientId.Equals(id));
    }

    public async Task<List<Client>> GetIndividualClientsFromBusinessId(Guid id, CancellationToken ct = default)
    {
        return await this.records
            .Include(c => c.User)
            .Include(c => c.JuristicClient)
            .Where(c => c.JuristicClientId.Equals(id))
            .ToListAsync(ct);
    }
}
