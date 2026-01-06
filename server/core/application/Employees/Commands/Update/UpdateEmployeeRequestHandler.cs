using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Update;

public class UpdateEmployeeRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryEmployee repositoryEmployee,
    IDistributedCache cache,
    IValidator<Employee> validator,
    ILogger<UpdateEmployeeRequestHandler> logger
) : IRequestHandler<UpdateEmployeeRequest, Result<UpdateEmployeeResponse>>
{
    public async Task<Result<UpdateEmployeeResponse>> Handle(
        UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        Employee? selectedEmployee = await repositoryEmployee.GetByIdAsync(request.Id);

        if (selectedEmployee is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Employee updatedEmployee = mapper.Map<Employee>(request);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedEmployee, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Employee> existingEmployees = await repositoryEmployee.GetAllAsync();

            if (DuplicateName(updatedEmployee, existingEmployees))
            {
                return Result.Fail(EmployeeErrorResults.DuplicateNameError(request.FullName));
            }

            await repositoryEmployee.UpdateAsync(request.Id, updatedEmployee);

            if (selectedEmployee.User is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("Something went wrong! The selected employee don't contains a user."));
            }

            selectedEmployee.User.FullName = request.FullName;

            await userManager.UpdateAsync(selectedEmployee.User);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("employees:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new UpdateEmployeeResponse(request.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }

    private static bool DuplicateName(Employee employee, List<Employee> existingEmployees)
    {
        return existingEmployees
            .Any(entity =>
            entity.Id != employee.Id &&
            string.Equals(
                entity.FullName,
                employee.FullName,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
