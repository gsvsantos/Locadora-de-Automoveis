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

    public async Task<bool> ExistsByDocumentAsync(string document)
    {
        return await this.records
            .AnyAsync(c =>
                c.Document != null &&
                c.Document.Equals(document)
            );
    }

    public async Task<bool> ExistsByDocumentAsync(string document, Guid ignoreClientId)
    {
        return await this.records
            .Where(c => !c.Id.Equals(ignoreClientId))
            .AnyAsync(c =>
                c.Document != null &&
                c.Document.Equals(document)
            );
    }

    public async Task<bool> UpdateGlobalAsync(Guid clientId, Client updatedClient)
    {
        Client? existingClient = await this.records
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c =>
                c.Id == clientId &&
                c.TenantId == null &&
                c.IsActive
            );

        if (existingClient is null)
        {
            return false;
        }

        existingClient.Update(updatedClient);

        return true;
    }

    public async Task<List<Client>> SearchAsync(string term, CancellationToken ct)
    {
        return await this.records
            .AsNoTracking()
            .Where(c =>
                c.FullName.ToLower().Contains(term) ||
                c.Email.ToLower().Contains(term) ||
                (c.Document != null && c.Document.ToLower().Contains(term))
            )
            .Take(5)
            .ToListAsync(ct);
    }

    public async Task<List<Client>> GetIndividualClientsFromBusinessIdAsync(Guid id, List<Guid> driversIds, CancellationToken ct = default)
    {
        return await this.records
            .Include(c => c.JuristicClient)
            .Where(c => c.JuristicClientId.Equals(id))
            .Where(c => !driversIds.Contains(c.Id))
            .Where(c => c.IsActive == true)
            .ToListAsync(ct);
    }

    public async Task<Client?> GetByTenantAndLoginUserIdAsync(Guid tenantId, Guid loginUserId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .Where(client =>
                client.IsActive == true &&
                client.TenantId.Equals(tenantId) &&
                client.LoginUserId.Equals(loginUserId)
            )
            .FirstOrDefaultAsync();
    }
    public async Task<Client?> GetGlobalByLoginUserIdAsync(Guid loginUserId)
    {
        return await this.records
            .IgnoreQueryFilters()
            .Where(client =>
                client.IsActive == true &&
                client.TenantId == null &&
                client.LoginUserId.Equals(loginUserId)
            )
            .FirstOrDefaultAsync();
    }

    public async Task<Client?> GetByTenantAndDocumentAsync(Guid tenantId, string document)
    {
        return await this.records
            .IgnoreQueryFilters()
            .Where(client =>
                client.IsActive &&
                client.TenantId.Equals(tenantId) &&
                client.Document == document)
            .FirstOrDefaultAsync();
    }
}
