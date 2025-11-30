using FluentResults;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;

public class GetByIdRentalRequestHandler(
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

            GetByIdRentalResponse response = new(
                new ByIdRentalDto(
                    selectedRental.Id,
                    (selectedRental.Employee is not null)
                        ? new RentalEmployeeDto(
                            selectedRental.Employee.Id,
                            selectedRental.Employee.FullName
                            )
                        : null,
                    new RentalClientDto(
                        selectedRental.ClientId,
                        selectedRental.Client.FullName
                        ),
                    new RentalDriverDto(
                        selectedRental.DriverId,
                        selectedRental.Driver.FullName
                        ),
                    new RentalVehicleDto(
                        selectedRental.VehicleId,
                        selectedRental.Vehicle.LicensePlate
                        ),
                    selectedRental.SelectedPlanType,
                    selectedRental.StartDate,
                    selectedRental.ExpectedReturnDate,
                    selectedRental.ReturnDate,
                    selectedRental.BaseRentalPrice,
                    selectedRental.FinalPrice,
                    selectedRental.RateServices.Select(rateService => new RentalRateServiceDto(
                        rateService.Id,
                        rateService.Name
                    )).ToImmutableList()
                )
            );

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
