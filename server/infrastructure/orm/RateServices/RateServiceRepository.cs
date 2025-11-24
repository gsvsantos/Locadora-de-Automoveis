using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Infrastructure.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.RateServices;

public class RateServiceRepository(AppDbContext context)
    : BaseRepository<RateService>(context), IRepositoryRateService;
