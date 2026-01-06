using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.Delete;

public class DeletePartnerRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryPartner repositoryPartner,
    IRepositoryCoupon repositoryCoupon,
    IDistributedCache cache,
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

        bool hasCoupons = await repositoryCoupon.ExistsByPartnerId(request.Id);

        try
        {
            if (hasCoupons)
            {
                selectedPartner.Deactivate();

                await repositoryPartner.UpdateAsync(selectedPartner.Id, selectedPartner);

                logger.LogInformation(
                    "Partner {PartnerId} was deactivated instead of deleted because it is in use.",
                    request.Id
                );
            }
            else
            {
                await repositoryPartner.DeleteAsync(request.Id);
            }

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("partners:master-version", Guid.NewGuid().ToString(), cancellationToken);

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
