using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.Delete;

public class DeleteCouponRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryCoupon repositoryCoupon,
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

        try
        {
            await repositoryCoupon.DeleteAsync(request.Id);

            await unitOfWork.CommitAsync();

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
