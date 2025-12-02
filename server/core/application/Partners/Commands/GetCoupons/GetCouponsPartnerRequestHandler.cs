using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Partners;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;

public class GetCouponsPartnerRequestHandler(
    IRepositoryPartner repositoryPartner,
    ILogger<GetCouponsPartnerRequestHandler> logger
) : IRequestHandler<GetCouponsPartnerRequest, Result<GetCouponsPartnerResponse>>
{
    public async Task<Result<GetCouponsPartnerResponse>> Handle(
        GetCouponsPartnerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Partner? selectedPartner = await repositoryPartner.GetByIdAsync(request.Id);

            if (selectedPartner is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetCouponsPartnerResponse response = new(
                new GetCouponsPartnerDto(
                    selectedPartner.Id,
                    selectedPartner.FullName,
                    selectedPartner.Coupons.Select(coupon => new CouponDto(
                        coupon.Id,
                        coupon.Name,
                        coupon.DiscountValue,
                        coupon.ExpirationDate
                    )).ToImmutableList()
                )
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving Pricing Plan by ID {PricingPlanId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the pricing plan."));
        }
    }
}
