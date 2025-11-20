using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Infrastructure.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.Employees;

public class EmployeeRepository(AppDbContext context)
    : BaseRepository<Employee>(context), IRepositoryEmployee;
