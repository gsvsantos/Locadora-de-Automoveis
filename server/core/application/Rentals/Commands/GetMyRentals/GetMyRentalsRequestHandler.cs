using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentals;

public class GetMyRentalsRequestHandler(
    IMapper mapper,
    IRepositoryRental repositoryRental,
    IUserContext userContext,
    ILogger<GetAllAvailableVehiclesRequestHandler> logger
) : IRequestHandler<GetMyRentalsRequest, Result<GetMyRentalsResponse>>
{
    public async Task<Result<GetMyRentalsResponse>> Handle(
        GetMyRentalsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid loginUserId = userContext.GetUserId();

            if (request.PageNumber < 1)
            {
                return Result.Fail(ErrorResults.BadRequestError("PageNumber must be greater than or equal to 1."));
            }

            if (request.PageSize is < 1 or > 100)
            {
                return Result.Fail(ErrorResults.BadRequestError("PageSize must be between 1 and 100."));
            }

            PagedResult<Rental> pagedRentals = await repositoryRental.GetMyRentalsDistinctAsync(
                loginUserId,
                request.PageNumber,
                request.PageSize,
                request.Term,
                request.TenantId,
                request.Status,
                cancellationToken
            );

            List<ClientRentalDto> rentalDtos = mapper.Map<List<ClientRentalDto>>(pagedRentals.Items);

            PagedResult<ClientRentalDto> result = new(
                rentalDtos,
                pagedRentals.TotalCount,
                pagedRentals.CurrentPage,
                pagedRentals.PageSize
            );

            GetMyRentalsResponse response = new(result);

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
