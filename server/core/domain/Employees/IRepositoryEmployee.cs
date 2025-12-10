using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Employees;

public interface IRepositoryEmployee : IRepository<Employee>
{
    Task<Employee?> GetByUserIdAsync(Guid userId);

    Task<List<Employee>> SearchAsync(string term, CancellationToken ct = default);
}
