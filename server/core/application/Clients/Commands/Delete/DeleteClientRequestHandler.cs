using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Delete;

public class DeleteClientRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryClient repositoryClient,
    IRepositoryRental repositoryRental,
    ILogger<DeleteClientRequestHandler> logger
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

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByClient(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot remove a client associated with active rentals."));
        }

        try
        {
            bool hasHistory = await repositoryRental.HasRentalHistoryByClient(request.Id);

            if (hasHistory)
            {
                selectedClient.Deactivate();

                await repositoryClient.UpdateAsync(selectedClient.Id, selectedClient);

                logger.LogInformation(
                    "Client '{@Name}' ({@Id}) was deactivated to preserve rental history.",
                    selectedClient.FullName,
                    request.Id
                );
            }
            else
            {
                await repositoryClient.DeleteAsync(request.Id);

                logger.LogInformation(
                    "Client '{@Name}' ({@Id}) was permanently deleted.",
                    selectedClient.FullName,
                    request.Id
                );
            }

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
