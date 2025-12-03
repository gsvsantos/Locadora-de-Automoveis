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
            List<Rental> rentals =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryRental.GetAllAsync(request.Quantity.Value)
                : await repositoryRental.GetAllAsync();

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
