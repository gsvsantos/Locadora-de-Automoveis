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
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.SelfUpdate;

public class SelfUpdateEmployeeRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryEmployee repositoryEmployee,
    IValidator<Employee> validator,
    ILogger<SelfUpdateEmployeeRequestHandler> logger
) : IRequestHandler<SelfUpdateEmployeeRequest, Result<SelfUpdateEmployeeResponse>>
{
    public async Task<Result<SelfUpdateEmployeeResponse>> Handle(
        SelfUpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Employee? selectedEmployee = await repositoryEmployee.GetByUserIdAsync(request.Id);

        if (selectedEmployee is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Employee updatedEmployee = mapper.Map<Employee>((request, selectedEmployee));

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

            await repositoryEmployee.UpdateAsync(selectedEmployee.Id, updatedEmployee);

            if (selectedEmployee.User is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("Something went wrong! The selected employee don't contains a user."));
            }

            selectedEmployee.User.FullName = request.FullName;

            await userManager.UpdateAsync(selectedEmployee.User);

            await unitOfWork.CommitAsync();

            return Result.Ok(new SelfUpdateEmployeeResponse(selectedEmployee.Id));
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
