using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalBy;

public class GetMyRentalByIdRequestHandler(
    IMapper mapper,
    IRepositoryRental repositoryRental,
    IUserContext userContext,
    ILogger<GetMyRentalByIdRequestHandler> logger
) : IRequestHandler<GetMyRentalByIdRequest, Result<GetMyRentalByIdResponse>>
{
    public async Task<Result<GetMyRentalByIdResponse>> Handle(
        GetMyRentalByIdRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid loginUserId = userContext.GetUserId();

            Rental? selectedRental = await repositoryRental.GetMyByIdDistinctAsync(request.Id, loginUserId);

            if (selectedRental is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetMyRentalByIdResponse response = mapper.Map<GetMyRentalByIdResponse>(selectedRental);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving Rental by ID {RentalId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the selectedRental."));
        }
    }
}
