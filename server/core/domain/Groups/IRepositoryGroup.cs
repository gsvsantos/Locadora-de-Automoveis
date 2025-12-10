using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Groups;

public interface IRepositoryGroup : IRepository<Group>
{
    Task<List<Group>> SearchAsync(string term, CancellationToken ct = default);
}
