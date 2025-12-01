using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.RateServices;

public interface IRepositoryRateService : IRepository<RateService>
{
    Task<List<RateService>> GetManyByIds(List<Guid> ids);
}