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

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Create;

public class CreateEmployeeRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryEmployee repositoryEmployee,
    ITenantProvider tenantProvider,
    IValidator<Employee> validator,
    ILogger<CreateEmployeeRequestHandler> logger
) : IRequestHandler<CreateEmployeeRequest, Result<CreateEmployeeResponse>>
{
    public async Task<Result<CreateEmployeeResponse>> Handle(
        CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        User user = new()
        {
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        IdentityResult usuarioResult = await userManager.CreateAsync(user, request.Password);

        if (!usuarioResult.Succeeded)
        {
            IEnumerable<string> erros = usuarioResult
                .Errors
                .Select(failure => failure.Description)
                .ToList();

            await userManager.DeleteAsync(user);

            return Result.Fail(ErrorResults.BadRequestError(erros));
        }

        await userManager.AddToRoleAsync(user, "Employee");

        Employee employee = new()
        {
            Id = user.Id,
            FullName = request.FullName,
            AdmissionDate = request.AdmissionDate,
            Salary = request.Salary
        };

        ValidationResult validationResult = await validator.ValidateAsync(employee);

        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors
                .Select(failure => failure.ErrorMessage)
                .ToList();

            return Result.Fail(ErrorResults.BadRequestError(errors));
        }

        List<Employee> existingEmployees = await repositoryEmployee.GetAllAsync();

        if (DuplicateName(employee, existingEmployees))
        {
            return Result.Fail(EmployeeErrorResults.DuplicateNameError(user.FullName));
        }

        {
            try
            {
                employee.AssociateUser(user);

                employee.AssociateTenant(tenantProvider.GetTenantId());

                await repositoryEmployee.AddAsync(employee);

                await unitOfWork.CommitAsync();

                return Result.Ok(new CreateEmployeeResponse(employee.Id));
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();

                await userManager.DeleteAsync(user);

                logger.LogError(
                    ex,
                    "An error occurred during registration. \n{@Request}.", request
                );

                return Result.Fail(ErrorResults.InternalServerError(ex));
            }
        }
    }

    private static bool DuplicateName(Employee employee, List<Employee> existingEmployees)
    {
        return existingEmployees
            .Any(entity => string.Equals(
                entity.FullName,
                employee.FullName,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
