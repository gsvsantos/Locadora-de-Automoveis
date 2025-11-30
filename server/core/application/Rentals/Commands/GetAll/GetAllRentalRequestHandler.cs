using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;

public class GetAllRentalRequestHandler(
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

            GetAllRentalResponse response = new(
                rentals.Count,
                rentals.Select(rental => new RentalDto(
                    rental.Id,
                    rental.Driver.FullName,
                    new VehicleDto(
                        rental.Vehicle.Id,
                        rental.Vehicle.LicensePlate,
                        rental.Vehicle.Brand,
                        rental.Vehicle.Color,
                        rental.Vehicle.Model,
                        rental.Vehicle.FuelType,
                       rental.Vehicle.FuelTankCapacity,
                        rental.Vehicle.Year,
                        rental.Vehicle.PhotoPath,
                        rental.Vehicle.GroupId
                    ),
                    rental.SelectedPlanType,
                    rental.StartDate,
                    rental.ExpectedReturnDate,
                    rental.ReturnDate,
                    rental.BaseRentalPrice,
                    rental.FinalPrice
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
