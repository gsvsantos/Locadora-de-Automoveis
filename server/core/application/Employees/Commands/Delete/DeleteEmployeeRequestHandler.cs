using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Delete;

public class DeleteEmployeeRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryEmployee repositoryEmployee,
    IRepositoryRental repositoryRental,
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

                if (selectedEmployee.User is not null)
                {
                    await userManager.SetLockoutEndDateAsync(selectedEmployee.User, DateTimeOffset.MaxValue);
                }

                await repositoryEmployee.UpdateAsync(selectedEmployee.Id, selectedEmployee);
            }
            else
            {
                await repositoryEmployee.DeleteAsync(request.Id);

                if (selectedEmployee.User is not null)
                {
                    await userManager.DeleteAsync(selectedEmployee.User);
                }
            }

            await unitOfWork.CommitAsync();

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
