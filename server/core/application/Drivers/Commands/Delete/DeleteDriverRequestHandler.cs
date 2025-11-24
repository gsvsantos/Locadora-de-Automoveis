using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.Delete;

public class DeleteDriverRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryDriver repositoryDriver,
    ILogger<DeleteDriverRequestHandler> logger
) : IRequestHandler<DeleteDriverRequest, Result<DeleteDriverResponse>>
{
    public async Task<Result<DeleteDriverResponse>> Handle(
        DeleteDriverRequest request, CancellationToken cancellationToken)
    {
        Driver? selectedDriver = await repositoryDriver.GetByIdAsync(request.Id);

        if (selectedDriver is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        try
        {
            await repositoryDriver.DeleteAsync(request.Id);

            await unitOfWork.CommitAsync();

            return Result.Ok(new DeleteDriverResponse());
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
