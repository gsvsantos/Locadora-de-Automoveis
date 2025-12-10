using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.Create;

public class CreateCouponRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryCoupon repositoryCoupon,
    IRepositoryPartner repositoryPartner,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<Coupon> validator,
    ILogger<CreateCouponRequestHandler> logger
) : IRequestHandler<CreateCouponRequest, Result<CreateCouponResponse>>
{
    public async Task<Result<CreateCouponResponse>> Handle(
        CreateCouponRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Partner? selectedPartner = await repositoryPartner.GetByIdAsync(request.PartnerId);

        if (selectedPartner is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.PartnerId));
        }

        Coupon coupon = mapper.Map<Coupon>(request);
        coupon.AssociatePartner(selectedPartner);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(coupon, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Coupon> existingCoupons = await repositoryCoupon.GetAllAsync();

            if (DuplicateName(coupon, existingCoupons))
            {
                return Result.Fail(CouponErrorResults.DuplicateNameError(request.Name));
            }

            coupon.AssociateTenant(tenantProvider.GetTenantId());

            coupon.AssociateUser(user);

            await repositoryCoupon.AddAsync(coupon);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreateCouponResponse(coupon.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }

    private static bool DuplicateName(Coupon coupon, List<Coupon> existingCoupons)
    {
        return existingCoupons
            .Any(entity => string.Equals(
                entity.Name,
                coupon.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
