using FluentResults;
using LocadoraDeAutomoveis.Application.Clients.Commands.Update;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Delete;

public class DeleteClientRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryClient repositoryClient,
    ILogger<UpdateClientRequestHandler> logger
) : IRequestHandler<DeleteClientRequest, Result<DeleteClientResponse>>
{
    public async Task<Result<DeleteClientResponse>> Handle(
        DeleteClientRequest request, CancellationToken cancellationToken)
    {
        Client? selectedClient = await repositoryClient.GetByIdAsync(request.Id);

        if (selectedClient is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        try
        {
            await repositoryClient.DeleteAsync(request.Id);

            await unitOfWork.CommitAsync();

            return Result.Ok(new DeleteClientResponse());
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
