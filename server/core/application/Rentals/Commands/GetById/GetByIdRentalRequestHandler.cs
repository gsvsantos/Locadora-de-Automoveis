using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;

public class GetByIdRentalRequestHandler(
    IMapper mapper,
    IRepositoryRental repositoryRental,
    ILogger<GetByIdRentalRequestHandler> logger
) : IRequestHandler<GetByIdRentalRequest, Result<GetByIdRentalResponse>>
{
    public async Task<Result<GetByIdRentalResponse>> Handle(
        GetByIdRentalRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Rental? selectedRental = await repositoryRental.GetByIdAsync(request.Id);

            if (selectedRental is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdRentalResponse response = mapper.Map<GetByIdRentalResponse>(selectedRental);

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
