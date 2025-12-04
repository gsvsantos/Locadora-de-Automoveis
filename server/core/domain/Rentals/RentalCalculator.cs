using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public static class RentalCalculator
{
    public static decimal CalculateBasePrice(Rental rental)
    {
        int estimatedDays = CalculateDays(rental.StartDate, rental.ExpectedReturnDate);

        decimal planTotal = CalculatePlanPrice(
            rental.BillingPlan, rental.SelectedPlanType,
            estimatedDays, 0m, rental.EstimatedKilometers ?? 0m
        );

        decimal servicesTotal = CalculateServicesPrice(rental.RateServices, estimatedDays);

        decimal grossTotal = planTotal + servicesTotal;

        decimal totalWithDiscount = ApplyCouponDiscount(grossTotal, rental.Coupon);

        return Math.Max(0m, totalWithDiscount);
    }

    public static CalculationResult CalculateFinalPrice(
        Rental rental,
        DateTimeOffset returnDate,
        decimal endKm,
        EFuelLevel fuelLevel,
        Configuration config)
    {
        int daysUsed = CalculateDays(rental.StartDate, returnDate);

        decimal kilometersDriven = Math.Max(0m, endKm - rental.StartKm);

        decimal planTotal = CalculatePlanPrice(
            rental.BillingPlan,
            rental.SelectedPlanType,
            daysUsed,
            kilometersDriven,
            rental.EstimatedKilometers ?? 0m
        );

        decimal servicesTotal = CalculateServicesPrice(rental.RateServices, daysUsed);

        decimal fuelPenalty = CalculateFuelPenalty(fuelLevel, rental.Vehicle, config);

        decimal delayPenalty = CalculateDelayPenalty(planTotal, rental.ExpectedReturnDate, returnDate);

        decimal penaltiesTotal = fuelPenalty + delayPenalty;

        decimal discountTotal = rental.Coupon?.DiscountValue ?? 0m;

        decimal grossTotal = planTotal + servicesTotal + penaltiesTotal;
        decimal finalPrice = Math.Max(0m, grossTotal - discountTotal);

        return new CalculationResult(
            finalPrice, planTotal,
            servicesTotal, penaltiesTotal,
            discountTotal, fuelPenalty
        );
    }

    private static int CalculateDays(DateTimeOffset start, DateTimeOffset end)
    {
        int days = (int)Math.Ceiling((end - start).TotalDays);
        return days <= 0 ? 1 : days;
    }

    private static decimal CalculatePlanPrice(
        BillingPlan plan,
        EBillingPlanType type,
        int days,
        decimal kmDriven,
        decimal estimatedKm)
    {
        return type switch
        {
            EBillingPlanType.Daily =>
                plan.DailyPlan.DailyRate * days + plan.DailyPlan.PricePerKm * kmDriven,

            EBillingPlanType.Controlled =>
                CalculateControlledPlan(plan.ControlledPlan, days, kmDriven, estimatedKm),

            EBillingPlanType.Free =>
                plan.FreePlan.FixedRate * days,

            _ => 0m
        };
    }

    private static decimal CalculateControlledPlan(
        ControlledPlanProps plan, int days,
        decimal kmDriven, decimal estimatedKm
    )
    {
        decimal baseCost = plan.DailyRate * days;
        decimal extraKm = Math.Max(0m, kmDriven - estimatedKm);

        return baseCost + extraKm * plan.PricePerKmExtrapolated;
    }

    private static decimal CalculateServicesPrice(List<RateService> services, int days)
    {
        if (services == null || services.Count == 0)
        {
            return 0m;
        }

        return services.Sum(service =>
            service.IsChargedPerDay ? service.Price * days : service.Price);
    }

    public static decimal CalculateFuelPenalty(EFuelLevel fuelLevel, Vehicle vehicle, Configuration config)
    {
        int levelPercent = (int)fuelLevel;
        if (levelPercent >= 100)
        {
            return 0m;
        }

        decimal missingLiters = vehicle.FuelTankCapacity * ((100 - levelPercent) / 100m);

        decimal pricePerLiter = vehicle.FuelType switch
        {
            EFuelType.Gasoline => config.GasolinePrice,
            EFuelType.Gas => config.GasPrice,
            EFuelType.Diesel => config.DieselPrice,
            EFuelType.Alcohol => config.AlcoholPrice,
            _ => 0m
        };

        return missingLiters * pricePerLiter;
    }

    private static decimal CalculateDelayPenalty(
        decimal planTotal,
        DateTimeOffset expectedReturn,
        DateTimeOffset actualReturn)
    {
        if (actualReturn.Date > expectedReturn.Date)
        {
            return planTotal * 0.10m;
        }

        return 0m;
    }

    private static decimal ApplyCouponDiscount(decimal total, Coupon? coupon)
    {
        if (coupon is null)
        {
            return total;
        }

        return total - coupon.DiscountValue;
    }
}

public record CalculationResult(
    decimal FinalPrice,
    decimal PlanTotal,
    decimal ServicesTotal,
    decimal PenaltiesTotal,
    decimal DiscountTotal,
    decimal FuelPenalty
);
