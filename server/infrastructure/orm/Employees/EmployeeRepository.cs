using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Employees;

public class EmployeeRepository(AppDbContext context)
    : BaseRepository<Employee>(context), IRepositoryEmployee
{
    public async Task<Employee?> GetByUserIdAsync(Guid userId)
    {
        return await this.records
            .FirstOrDefaultAsync(e => e.UserId == userId);
    }
}
