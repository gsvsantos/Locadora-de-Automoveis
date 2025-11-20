using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Employees;

public class EmployeeRepository(AppDbContext context)
    : BaseRepository<Employee>(context), IRepositoryEmployee
{
    public override async Task<Employee?> GetByIdAsync(Guid entityId) => await this.records
        .Include(e => e.User).FirstOrDefaultAsync(x => x.Id.Equals(entityId));
}
