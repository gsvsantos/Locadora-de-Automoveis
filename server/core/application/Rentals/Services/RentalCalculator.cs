using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Application.Rentals.Services;

public static class RentalCalculator
{
    public static decimal CalculateServicesCost(List<RateService> rateServices, int daysUsed)
    {
        return rateServices.Sum(service =>
                service.IsChargedPerDay
                    ? service.Price * daysUsed
                    : service.Price);
    }

    public static decimal CalculateFuelPenalty(EFuelLevel fuelLevelEnum, Vehicle vehicle, Configuration config)
    {
        int levelPercent = (int)fuelLevelEnum;

        if (levelPercent >= 100)
        {
            return 0;
        }

        decimal capacity = vehicle.FuelTankCapacity;
        decimal currentLiters = capacity * (levelPercent / 100m);
        decimal missingLiters = capacity - currentLiters;

        decimal pricePerLiter = vehicle.FuelType switch
        {
            EFuelType.Gasoline => config.GasolinePrice,
            EFuelType.Gas => config.GasPrice,
            EFuelType.Diesel => config.DieselPrice,
            EFuelType.Alcohol => config.AlcoholPrice,
            _ => 0
        };

        return missingLiters * pricePerLiter;
    }

    public static decimal CalculateRentalPlansCost(Rental rental, int daysRented, decimal kilometersDriven, decimal? estimatedKilometers)
    {
        AvailablePlan planProps = rental.SelectedPlanType switch
        {
            EPricingPlanType.Daily => rental.PricingPlan.DailyPlan,
            EPricingPlanType.Controlled => rental.PricingPlan.ControlledPlan,
            EPricingPlanType.Free => rental.PricingPlan.FreePlan,
            _ => throw new InvalidOperationException("The chosed plan is invalid.")
        };

        return planProps switch
        {
            DailyPlanProps p => p.DailyRate * daysRented + p.PricePerKm * kilometersDriven,

            ControlledPlanProps p => CalculateControlledPlan(p, daysRented, kilometersDriven, estimatedKilometers),

            FreePlanProps p => p.FixedRate,

            _ => throw new ArgumentException("Unknown plan type")
        };
    }

    private static decimal CalculateControlledPlan(ControlledPlanProps plan, int days, decimal kilometersDriven, decimal? estimatedKilometers)
    {
        if (estimatedKilometers is null)
        {
            throw new InvalidOperationException("Estimated kilometers must be provided for controlled plan.");
        }

        decimal baseCost = plan.DailyRate * days;
        decimal extraKilometers = Math.Max(0, kilometersDriven - estimatedKilometers.Value);
        decimal extraCost = extraKilometers * plan.PricePerKmExtrapolated;

        return baseCost + extraCost;
    }
}