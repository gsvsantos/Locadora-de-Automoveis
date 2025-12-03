using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Partners;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;

public class GetCouponsPartnerRequestHandler(
    IMapper mapper,
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

            GetCouponsPartnerResponse response = mapper.Map<GetCouponsPartnerResponse>(selectedPartner);

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
