using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.Delete;

public class DeleteCouponRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryCoupon repositoryCoupon,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    ILogger<DeleteCouponRequestHandler> logger
) : IRequestHandler<DeleteCouponRequest, Result<DeleteCouponResponse>>
{
    public async Task<Result<DeleteCouponResponse>> Handle(
        DeleteCouponRequest request, CancellationToken cancellationToken)
    {
        Coupon? selectedCoupon = await repositoryCoupon.GetByIdAsync(request.Id);

        if (selectedCoupon is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByCoupon(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot remove a coupon associated with active rentals."));
        }

        try
        {
            bool hasHistory = await repositoryRental.HasRentalHistoryByCoupon(request.Id);

            if (hasHistory)
            {
                selectedCoupon.Deactivate();

                await repositoryCoupon.UpdateAsync(selectedCoupon.Id, selectedCoupon);

                logger.LogInformation("Coupon '{@Name}' was deactivated to preserve rental history.",
                    selectedCoupon.Name
                );
            }
            else
            {
                await repositoryCoupon.DeleteAsync(request.Id);

                logger.LogInformation("Coupon '{@Name}' was permanently deleted.",
                    selectedCoupon.Name
                );
            }

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("coupons:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new DeleteCouponResponse());
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
