using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;

public class GetAllRentalExtraRequestHandler(
    IMapper mapper,
    IRepositoryRentalExtra repositoryRentalExtra,
    ILogger<GetAllRentalExtraRequestHandler> logger
) : IRequestHandler<GetAllRentalExtraRequest, Result<GetAllRentalExtraResponse>>
{
    public async Task<Result<GetAllRentalExtraResponse>> Handle(
        GetAllRentalExtraRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<RentalExtra> rentalExtras = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                rentalExtras = await repositoryRentalExtra.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                rentalExtras = await repositoryRentalExtra.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                rentalExtras = await repositoryRentalExtra.GetAllAsync(quantity);
            }
            else
            {
                rentalExtras = await repositoryRentalExtra.GetAllAsync(true);
            }

            GetAllRentalExtraResponse response = mapper.Map<GetAllRentalExtraResponse>(rentalExtras);

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
