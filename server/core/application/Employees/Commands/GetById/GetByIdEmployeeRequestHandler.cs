using FluentResults;
using LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Employees;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetById;

public class GetByIdEmployeeRequestHandler(
    IRepositoryEmployee repositoryEmployee,
    ILogger<GetByIdEmployeeRequestHandler> logger
) : IRequestHandler<GetByIdEmployeeRequest, Result<GetByIdEmployeeResponse>>
{
    public async Task<Result<GetByIdEmployeeResponse>> Handle(
        GetByIdEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Employee? employee = await repositoryEmployee.GetByIdAsync(request.Id);

            if (employee == null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdEmployeeResponse response = new(
                 new EmployeeDto(
                    employee.Id,
                    employee.FullName,
                    employee.AdmissionDate,
                    employee.Salary
                )
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving employee by ID {EmployeeId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the employee."));
        }
    }
}
