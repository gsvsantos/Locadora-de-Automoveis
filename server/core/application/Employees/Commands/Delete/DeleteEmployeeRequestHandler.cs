using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Delete;

public class DeleteEmployeeRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryEmployee repositoryEmployee,
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

        try
        {
            // todo: verificação de relações -> tem relações ?? desativa : delete
            //selectedEmployee.Deactivate();
            //await repositoryEmployee.UpdateAsync(request.Id, selectedEmployee);
            //await userManager.UpdateAsync(selectedEmployee.User);

            await repositoryEmployee.DeleteAsync(request.Id);

            await userManager.DeleteAsync(selectedEmployee.User);

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
