using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public static class RentalCalculator
{
    public static decimal CalculateBasePrice(Rental rental)
    {
        int estimatedDays = CalculateDays(rental.StartDate, rental.ExpectedReturnDate);

        decimal planTotalCost = CalculatePlanPrice(
            rental.BillingPlan, rental.BillingPlanType,
            estimatedDays, 0m, rental.EstimatedKilometers ?? 0m
        );

        decimal extrasTotalCost = CalculateExtrasCost(rental.Extras, estimatedDays);

        decimal totalCost = planTotalCost + extrasTotalCost;

        decimal totalWithDiscount = ApplyCouponDiscount(totalCost, rental.Coupon);

        return Math.Max(0m, totalWithDiscount);
    }

    public static CalculationResult CalculateFinalPrice(
        Rental rental,
        DateTimeOffset returnDate,
        decimal endKm,
        EFuelLevel fuelLevelAtReturn,
        Configuration config)
    {
        int daysUsed = CalculateDays(rental.StartDate, returnDate);

        decimal kilometersDriven = Math.Max(0m, endKm - rental.StartKm);
        rental.Vehicle.KilometersSum(kilometersDriven);

        decimal planTotalCost = CalculatePlanPrice(
            rental.BillingPlan,
            rental.BillingPlanType,
            daysUsed,
            kilometersDriven,
            rental.EstimatedKilometers ?? 0m
        );

        decimal extrasTotalCost = CalculateExtrasCost(rental.Extras, daysUsed);

        decimal fuelPenalty = CalculateFuelPenalty(fuelLevelAtReturn, rental.Vehicle, config);

        decimal delayPenalty = CalculateDelayPenalty(planTotalCost, rental.ExpectedReturnDate, returnDate);

        decimal discountTotal = rental.Coupon?.DiscountValue ?? 0m;

        planTotalCost = Money(planTotalCost);
        extrasTotalCost = Money(extrasTotalCost);
        fuelPenalty = Money(fuelPenalty);
        delayPenalty = Money(delayPenalty);
        discountTotal = Money(discountTotal);

        decimal penaltiesTotalCost = Money(fuelPenalty + delayPenalty);

        decimal totalCost = Money(planTotalCost + extrasTotalCost + penaltiesTotalCost);
        decimal finalPrice = Money(Math.Max(0m, totalCost - discountTotal));

        return new CalculationResult(
            finalPrice,
            planTotalCost,
            extrasTotalCost,
            penaltiesTotalCost,
            discountTotal,
            fuelPenalty,
            delayPenalty
        );
    }

    private static int CalculateDays(DateTimeOffset start, DateTimeOffset end)
    {
        int days = (int)Math.Ceiling((end.Date - start.Date).TotalDays);
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
                plan.Daily.DailyRate * days + plan.Daily.PricePerKm * kmDriven,

            EBillingPlanType.Controlled =>
                CalculateControlledPlan(plan.Controlled, days, kmDriven, estimatedKm),

            EBillingPlanType.Free =>
                plan.Free.FixedRate * days,

            _ => 0m
        };
    }

    private static decimal CalculateControlledPlan(
        ControlledBilling plan, int days,
        decimal kmDriven, decimal estimatedKm
    )
    {
        decimal baseCost = plan.DailyRate * days;
        decimal extraKm = Math.Max(0m, kmDriven - estimatedKm);

        return baseCost + extraKm * plan.PricePerKmExtrapolated;
    }

    private static decimal CalculateExtrasCost(List<RentalExtra> extras, int days)
    {
        if (extras == null || extras.Count == 0)
        {
            return 0m;
        }

        return extras.Sum(extra =>
            extra.IsDaily ? extra.Price * days : extra.Price);
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
        DateTimeOffset actualReturn
    )
    {
        return actualReturn.Date > expectedReturn.Date ? planTotal * 0.10m : 0;
    }

    private static decimal ApplyCouponDiscount(decimal total, Coupon? coupon)
    {
        if (coupon is null)
        {
            return total;
        }

        return total - coupon.DiscountValue;
    }

    private static decimal Money(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}

public record CalculationResult(
    decimal FinalPrice,
    decimal PlanTotal,
    decimal ServicesTotal,
    decimal PenaltiesTotal,
    decimal DiscountTotal,
    decimal FuelPenalty,
    decimal DelayPenalty
);
