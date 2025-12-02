using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.Delete;

public class DeletePartnerRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryPartner repositoryPartner,
    ILogger<DeletePartnerRequestHandler> logger
) : IRequestHandler<DeletePartnerRequest, Result<DeletePartnerResponse>>
{
    public async Task<Result<DeletePartnerResponse>> Handle(
        DeletePartnerRequest request, CancellationToken cancellationToken)
    {
        Partner? selectedPartner = await repositoryPartner.GetByIdAsync(request.Id);

        if (selectedPartner is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        try
        {
            await repositoryPartner.DeleteAsync(request.Id);

            await unitOfWork.CommitAsync();

            return Result.Ok(new DeletePartnerResponse());

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
