using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;

public class GetAllRentalRequestHandler(
    IMapper mapper,
    IRepositoryRental repositoryRental,
    ILogger<GetAllRentalRequestHandler> logger
) : IRequestHandler<GetAllRentalRequest, Result<GetAllRentalResponse>>
{
    public async Task<Result<GetAllRentalResponse>> Handle(
        GetAllRentalRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Rental> rentals = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                rentals = await repositoryRental.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                rentals = await repositoryRental.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                rentals = await repositoryRental.GetAllAsync(quantity);
            }
            else
            {
                rentals = await repositoryRental.GetAllAsync(true);
            }

            GetAllRentalResponse response = mapper.Map<GetAllRentalResponse>(rentals);

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
