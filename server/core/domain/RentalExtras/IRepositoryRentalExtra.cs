using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.RentalExtras;

public interface IRepositoryRentalExtra : IRepository<RentalExtra>
{
    Task<List<RentalExtra>> GetManyByIds(List<Guid> ids);
}