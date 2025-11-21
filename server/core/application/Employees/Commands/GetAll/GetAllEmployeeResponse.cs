namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public record GetAllEmployeeResponse(
    int Quantity,
    IEnumerable<EmployeeDto> Employees
);

public record EmployeeDto(
    Guid Id,
    string FullName,
    DateTimeOffset AdmissionDate,
    decimal Salary
);
