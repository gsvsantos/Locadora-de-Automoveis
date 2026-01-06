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
            .Where(c => c.IsActive == true)
            .FirstOrDefaultAsync(e => e.UserId.Equals(userId));
    }

    public async Task<List<Employee>> SearchAsync(string term, CancellationToken ct)
    {
        return await this.records
            .AsNoTracking()
            .Where(e =>
                e.FullName.ToLower().Contains(term) ||
                e.User!.Email!.ToLower().Contains(term)
            )
            .Take(5)
            .ToListAsync(ct);
    }

    public override async Task<Employee?> GetByIdAsync(Guid entityId)
    {
        return await this.records
            .Include(d => d.LoginUser)
            .FirstOrDefaultAsync(d => d.Id.Equals(entityId));
    }
}
