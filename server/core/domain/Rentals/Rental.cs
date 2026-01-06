using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public class Rental : BaseEntity<Rental>
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset ExpectedReturnDate { get; set; }
    public decimal StartKm { get; set; } = 0;
    public decimal BaseRentalPrice { get; set; } = 0;
    public decimal GuaranteeValue { get; set; } = 1000;
    public ERentalStatus Status { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }
    public RentalReturn? RentalReturn { get; set; }
    public decimal FinalPrice { get; set; } = 0;
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public Guid DriverId { get; set; }
    public Driver Driver { get; set; } = null!;
    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public Guid? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
    public Guid BillingPlanId { get; set; }
    public BillingPlan BillingPlan { get; set; } = null!;
    public EBillingPlanType BillingPlanType { get; set; }
    public decimal? EstimatedKilometers { get; set; }
    public List<RentalExtra> Extras { get; set; } = [];

    public Rental() { }
    public Rental(DateTimeOffset startDate, DateTimeOffset expectedReturnDate) : this()
    {
        this.StartDate = startDate;
        this.ExpectedReturnDate = expectedReturnDate;
    }

    public void AddRangeExtras(List<RentalExtra> extras)
    {
        this.Extras.AddRange(extras);
    }

    public void AddExtra(RentalExtra extra)
    {
        this.Extras.Add(extra);
    }

    public void RemoveExtra(RentalExtra extra)
    {
        this.Extras.Remove(extra);
    }

    public void SetBasePrice(decimal basePrice)
    {
        this.BaseRentalPrice = basePrice;
    }

    public void SetEstimatedKilometers(decimal estimatedKilometers)
    {
        this.EstimatedKilometers = estimatedKilometers;
    }

    public void SetStartKm(decimal startKm)
    {
        this.StartKm = startKm;
    }

    public void SetStatus(ERentalStatus status)
    {
        this.Status = status;
    }

    public void SetReturnDate(DateTimeOffset returnDate)
    {
        this.ReturnDate = returnDate;
    }

    public void SetBillingPlanType(EBillingPlanType selectedPlanType)
    {
        this.BillingPlanType = selectedPlanType;
    }

    public void SetFinalPrice(decimal finalPrice)
    {
        this.FinalPrice = finalPrice;
    }

    public void AssociateEmployee(Employee employee)
    {
        this.Employee = employee;
        this.EmployeeId = employee.Id;
    }

    public void AssociateClient(Client client)
    {
        this.Client = client;
        this.ClientId = client.Id;
    }

    public void AssociateDriver(Driver driver)
    {
        this.Driver = driver;
        this.DriverId = driver.Id;
    }

    public void AssociateVehicle(Vehicle vehicle)
    {
        this.Vehicle = vehicle;
        this.VehicleId = vehicle.Id;
    }

    public void AssociateCoupon(Coupon coupon)
    {
        this.Coupon = coupon;
        this.CouponId = coupon.Id;
    }

    public void AssociateBillingPlan(BillingPlan BillingPlan)
    {
        this.BillingPlan = BillingPlan;
        this.BillingPlanId = BillingPlan.Id;
    }

    public override void Update(Rental updatedEntity)
    {
        this.StartDate = updatedEntity.StartDate;
        this.ExpectedReturnDate = updatedEntity.ExpectedReturnDate;
        this.StartKm = updatedEntity.StartKm;

        if (updatedEntity.Employee is not null)
        {
            AssociateEmployee(updatedEntity.Employee);
        }

        if (updatedEntity.Coupon is not null)
        {
            AssociateCoupon(updatedEntity.Coupon);
        }

        AssociateClient(updatedEntity.Client);
        AssociateDriver(updatedEntity.Driver);
        AssociateVehicle(updatedEntity.Vehicle);
        AssociateBillingPlan(updatedEntity.BillingPlan);

        SetBillingPlanType(updatedEntity.BillingPlanType);

        AddRangeExtras(updatedEntity.Extras);

        if (updatedEntity.EstimatedKilometers.HasValue)
        {
            SetEstimatedKilometers(updatedEntity.EstimatedKilometers.Value);
        }
    }
}

public enum ERentalStatus
{
    Open,
    Completed,
    Canceled
}

public enum EBillingPlanType
{
    Daily,
    Controlled,
    Free
}