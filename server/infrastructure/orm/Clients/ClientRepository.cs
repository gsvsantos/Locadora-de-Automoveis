using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Clients;

public class ClientRepository(AppDbContext context)
    : BaseRepository<Client>(context), IRepositoryClient
{
    public override Task<List<Client>> GetAllAsync() =>
        this.records.Include(c => c.Driver).ToListAsync();

    public override Task<List<Client>> GetAllAsync(int quantity) =>
        this.records.Include(c => c.Driver).Take(quantity).ToListAsync();

    public override Task<Client?> GetByIdAsync(Guid entityId) =>
        this.records.Include(c => c.Driver).FirstOrDefaultAsync();
}
