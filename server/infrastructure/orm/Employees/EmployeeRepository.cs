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

    public async Task<List<Employee>> SearchAsync(string term, CancellationToken ct)
    {
        return await this.records
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e =>
                e.FullName.ToLower().Contains(term) ||
                e.User!.Email!.ToLower().Contains(term)
            )
            .Take(5)
            .ToListAsync(ct);
    }
}
