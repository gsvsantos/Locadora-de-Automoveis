using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.Update;

public class UpdateCouponRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryCoupon repositoryCoupon,
    IRepositoryPartner repositoryPartner,
    IDistributedCache cache,
    IValidator<Coupon> validator,
    ILogger<UpdateCouponRequestHandler> logger
) : IRequestHandler<UpdateCouponRequest, Result<UpdateCouponResponse>>
{
    public async Task<Result<UpdateCouponResponse>> Handle(
        UpdateCouponRequest request, CancellationToken cancellationToken)
    {
        Coupon? selectedCoupon = await repositoryCoupon.GetByIdAsync(request.Id);

        if (selectedCoupon is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Partner? selectedPartner = await repositoryPartner.GetByIdAsync(request.PartnerId);

        if (selectedPartner is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.PartnerId));
        }

        Coupon updatedCoupon = mapper.Map<Coupon>(request);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedCoupon, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            selectedCoupon.AssociatePartner(selectedPartner);

            List<Coupon> existingCoupons = await repositoryCoupon.GetAllAsync();

            if (DuplicateName(selectedCoupon, existingCoupons))
            {
                return Result.Fail(CouponErrorResults.DuplicateNameError(request.Name));
            }

            await repositoryCoupon.UpdateAsync(selectedCoupon.Id, updatedCoupon);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("coupons:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new UpdateCouponResponse(selectedCoupon.Id));
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

    private static bool DuplicateName(Coupon coupon, List<Coupon> existingCoupons)
    {
        return existingCoupons
            .Any(entity =>
            entity.Id != coupon.Id &&
            string.Equals(
                entity.Name,
                coupon.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
