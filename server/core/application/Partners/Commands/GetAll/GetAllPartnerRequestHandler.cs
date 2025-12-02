using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Partners;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;

public class GetAllPartnerRequestHandler(
    IRepositoryPartner repositoryPartner,
    ILogger<GetAllPartnerRequestHandler> logger
) : IRequestHandler<GetAllPartnerRequest, Result<GetAllPartnerResponse>>
{
    public async Task<Result<GetAllPartnerResponse>> Handle(
        GetAllPartnerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Partner> partners =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryPartner.GetAllAsync(request.Quantity.Value)
                : await repositoryPartner.GetAllAsync();

            GetAllPartnerResponse response = new(
                partners.Count,
                partners.Select(partner => new GetAllPartnerDto(
                    partner.Id,
                    partner.FullName,
                    partner.Coupons.Count
                )).ToImmutableList()
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
