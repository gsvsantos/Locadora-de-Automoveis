using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Drivers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;

public class GetAllDriverRequestHandler(
    IMapper mapper,
    IRepositoryDriver repositoryDriver,
    ILogger<GetAllDriverRequestHandler> logger
) : IRequestHandler<GetAllDriverRequest, Result<GetAllDriverResponse>>
{
    public async Task<Result<GetAllDriverResponse>> Handle(
        GetAllDriverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Driver> drivers = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                drivers = await repositoryDriver.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                drivers = await repositoryDriver.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                drivers = await repositoryDriver.GetAllAsync(quantity);
            }
            else
            {
                drivers = await repositoryDriver.GetAllAsync(true);
            }

            GetAllDriverResponse response = mapper.Map<GetAllDriverResponse>(drivers);

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
