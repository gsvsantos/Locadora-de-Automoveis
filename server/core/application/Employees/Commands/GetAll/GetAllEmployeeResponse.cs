namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public record GetAllEmployeeResponse(
    int Quantity,
    IEnumerable<GetAllEmployeeDto> Employees
);

public record GetAllEmployeeDto(
    Guid Id,
    string FullName,
    DateTimeOffset AdmissionDate,
    decimal Salary
);
