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

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Create;

public class CreateEmployeeRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryEmployee repositoryEmployee,
    IDistributedCache cache,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<Employee> validator,
    ILogger<CreateEmployeeRequestHandler> logger
) : IRequestHandler<CreateEmployeeRequest, Result<CreateEmployeeResponse>>
{
    public async Task<Result<CreateEmployeeResponse>> Handle(
        CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        User? currentUser = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (currentUser is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        User userLogin = mapper.Map<User>(request);

        try
        {
            IdentityResult usuarioResult = await userManager.CreateAsync(userLogin, request.Password);

            if (!usuarioResult.Succeeded)
            {
                IEnumerable<string> erros = usuarioResult
                    .Errors
                    .Select(failure => failure.Description)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(erros));
            }
            await userManager.AddToRoleAsync(userLogin, "Employee");

            userLogin.AssociateTenant(tenantProvider.GetTenantId());

            await userManager.UpdateAsync(userLogin);

            Employee employee = mapper.Map<Employee>(request);

            ValidationResult validationResult = await validator.ValidateAsync(employee, cancellationToken);

            if (!validationResult.IsValid)
            {
                await userManager.DeleteAsync(userLogin);

                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Employee> existingEmployees = await repositoryEmployee.GetAllAsync();

            if (DuplicateName(employee, existingEmployees))
            {
                await userManager.DeleteAsync(userLogin);

                return Result.Fail(EmployeeErrorResults.DuplicateNameError(request.FullName));
            }

            employee.AssociateLoginUser(userLogin);
            employee.AssociateUser(currentUser);
            employee.AssociateTenant(tenantProvider.GetTenantId());

            await repositoryEmployee.AddAsync(employee);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("employees:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new CreateEmployeeResponse(employee.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            if (userLogin.Id != Guid.Empty)
            {
                await userManager.DeleteAsync(userLogin);
            }

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
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
