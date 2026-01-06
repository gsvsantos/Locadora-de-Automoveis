using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Configurations;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Return;

public class ReturnRentalRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryRentalReturn repositoryRentalReturn,
    IRepositoryRental repositoryRental,
    IRepositoryConfiguration repositoryConfiguration,
    IDistributedCache cache,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<RentalReturn> validator,
    ILogger<ReturnRentalRequestHandler> logger
) : IRequestHandler<ReturnRentalRequest, Result<ReturnRentalResponse>>
{
    public async Task<Result<ReturnRentalResponse>> Handle(
        ReturnRentalRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Guid tenantId = tenantProvider.GetTenantId();
        Configuration? configuration = await repositoryConfiguration.GetByTenantId(tenantId);

        if (configuration is null)
        {
            return Result.Fail(ConfigurationErrorResults.NotFoundForTenant(tenantId));
        }

        Rental? selectedRental = await repositoryRental.GetByIdAsync(request.Id);

        if (selectedRental is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        if (selectedRental.ReturnDate.HasValue)
        {
            return Result.Fail(RentalErrorResults.RentalAlreadyBeenReturned(selectedRental.ReturnDate.Value));
        }

        DateTimeOffset returnDate = DateTimeOffset.UtcNow;

        int daysUsed = (int)Math.Ceiling((returnDate.Date - selectedRental.StartDate.Date).TotalDays);
        if (daysUsed <= 0)
        {
            daysUsed = 1;
        }

        decimal kmDriven = request.EndKm - selectedRental.StartKm;

        if (kmDriven < 0)
        {
            return Result.Fail(ErrorResults.BadRequestError("The final odometer reading cannot be less than the initial reading."));
        }

        RentalReturn rentalReturn = mapper.Map<RentalReturn>((request, returnDate, kmDriven));

        rentalReturn.SetFuelLevel(request.FuelLevelAtReturn);
        rentalReturn.AssociateRental(selectedRental);
        rentalReturn.SetDaysUsed(daysUsed);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(rentalReturn, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            CalculationResult calculationResult = RentalCalculator.CalculateFinalPrice(
                selectedRental,
                returnDate,
                request.EndKm,
                rentalReturn.FuelLevelAtReturn,
                configuration
            );

            rentalReturn.SetExtrasTotalCost(calculationResult.ServicesTotal);
            rentalReturn.SetFuelPenalty(calculationResult.FuelPenalty);
            rentalReturn.SetDelayPenalty(calculationResult.DelayPenalty);
            rentalReturn.SetPenaltyTotal(calculationResult.PenaltiesTotal);
            rentalReturn.SetDiscountTotal(calculationResult.DiscountTotal);
            rentalReturn.SetFinalPrice(calculationResult.FinalPrice);

            selectedRental.SetStatus(ERentalStatus.Completed);
            selectedRental.SetReturnDate(returnDate);
            selectedRental.SetFinalPrice(calculationResult.FinalPrice);

            rentalReturn.AssociateTenant(tenantId);
            rentalReturn.AssociateUser(user);

            await repositoryRentalReturn.AddAsync(rentalReturn);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("rentals:master-version", Guid.NewGuid().ToString(), cancellationToken);
            await cache.SetStringAsync("vehicles:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new ReturnRentalResponse(rentalReturn.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
