using FluentResults;
using LocadoraDeAutomoveis.Domain.Employees;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public class GetAllEmployeeRequestHandler(
    IRepositoryEmployee repositoryEmployee,
    ILogger<GetAllEmployeeRequestHandler> logger
) : IRequestHandler<GetAllEmployeeRequest, Result<GetAllEmployeeResponse>>
{
    public async Task<Result<GetAllEmployeeResponse>> Handle(
        GetAllEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Employee> employees = await repositoryEmployee.GetAllAsync();

            GetAllEmployeeResponse response = new(
                employees.Count(),
                employees.Select(employee => new EmployeeDto(
                    employee.Id,
                    employee.FullName,
                    employee.AdmissionDate,
                    employee.Salary
                ))
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all employees");
            return Result.Fail(new Error("Error retrieving all employees").CausedBy(ex));
        }
    }
}
