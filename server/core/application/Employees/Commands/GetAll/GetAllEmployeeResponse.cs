using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public record GetAllEmployeeResponse(
    int Quantity,
    ImmutableList<EmployeeDto> Employees
);

public record EmployeeDto(
    Guid Id,
    string FullName,
    DateTimeOffset AdmissionDate,
    decimal Salary,
    bool IsActive
);
