using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Delete;

public class DeleteEmployeeRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryEmployee repositoryEmployee,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    IAuthEmailService authEmailService,
    ILogger<DeleteEmployeeRequestHandler> logger
) : IRequestHandler<DeleteEmployeeRequest, Result<DeleteEmployeeResponse>>
{
    public async Task<Result<DeleteEmployeeResponse>> Handle(
        DeleteEmployeeRequest request, CancellationToken cancellationToken)
    {
        Employee? selectedEmployee = await repositoryEmployee.GetByIdAsync(request.Id);

        if (selectedEmployee is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByEmployee(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot remove employee associated with active rentals."));
        }

        try
        {
            bool hasHistory = await repositoryRental.HasRentalHistoryByEmployee(request.Id);
            if (hasHistory)
            {
                selectedEmployee.Deactivate();

                if (selectedEmployee.LoginUser is not null)
                {
                    await userManager.SetLockoutEndDateAsync(selectedEmployee.LoginUser, DateTimeOffset.MaxValue);

                    try
                    {
                        await authEmailService.ScheduleAccountDeactivationNotice(selectedEmployee.LoginUser.Email!, selectedEmployee.FullName, selectedEmployee.LoginUser.PreferredLanguage);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(
                            ex,
                            "Failed to send deactivation email for user {UserId}", selectedEmployee.LoginUser.Id
                        );
                    }

                    logger.LogInformation(
                        "User account '{Email}' (ID: {UserId}) has been locked indefinitely due to employee deactivation.",
                        selectedEmployee.LoginUser.Email,
                        selectedEmployee.LoginUser.Id
                    );
                }

                await repositoryEmployee.UpdateAsync(selectedEmployee.Id, selectedEmployee);

                await unitOfWork.CommitAsync();

                logger.LogInformation(
                    "Employee {@EmployeeId} was deactivated (Soft Delete) instead of permanently deleted to preserve rental history.",
                    request.Id
                );
            }
            else
            {
                await repositoryEmployee.DeleteAsync(request.Id);

                await unitOfWork.CommitAsync();

                if (selectedEmployee.LoginUser is not null)
                {
                    IdentityResult deleteResult = await userManager.DeleteAsync(selectedEmployee.LoginUser);

                    if (!deleteResult.Succeeded)
                    {
                        IEnumerable<string> errors = deleteResult.Errors
                        .Select(failure => failure.Description)
                        .ToList();

                        return Result.Fail(ErrorResults.BadRequestError(errors));
                    }
                }
            }

            await cache.SetStringAsync("employees:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new DeleteEmployeeResponse());
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
}
