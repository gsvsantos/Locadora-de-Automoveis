using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Infrastructure.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.Configurations;

public class ConfigurationRepository(AppDbContext context)
    : BaseRepository<Configuration>(context), IRepositoryConfiguration;
