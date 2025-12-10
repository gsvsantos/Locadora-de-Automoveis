using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Partners;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;

public class GetAllPartnerRequestHandler(
    IMapper mapper,
    IRepositoryPartner repositoryPartner,
    ILogger<GetAllPartnerRequestHandler> logger
) : IRequestHandler<GetAllPartnerRequest, Result<GetAllPartnerResponse>>
{
    public async Task<Result<GetAllPartnerResponse>> Handle(
        GetAllPartnerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Partner> partners = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                partners = await repositoryPartner.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                partners = await repositoryPartner.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                partners = await repositoryPartner.GetAllAsync(quantity);
            }
            else
            {
                partners = await repositoryPartner.GetAllAsync(true);
            }

            GetAllPartnerResponse response = mapper.Map<GetAllPartnerResponse>(partners);

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
