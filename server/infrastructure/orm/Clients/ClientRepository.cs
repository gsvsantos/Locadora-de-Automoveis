using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Infrastructure.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.Clients;

public class ClientRepository(AppDbContext context)
    : BaseRepository<Client>(context), IRepositoryClient;
