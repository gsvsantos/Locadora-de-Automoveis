using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.RateServices;
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
    public decimal FinalPrice { get; set; } = 0;
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public Guid DriverId { get; set; }
    public Driver Driver { get; set; } = null!;
    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public Guid PricingPlanId { get; set; }
    public PricingPlan PricingPlan { get; set; } = null!;
    public EPricingPlanType SelectedPlanType { get; set; }
    public decimal? EstimatedKilometers { get; set; }
    public List<RateService> RateServices { get; set; } = [];

    public Rental() { }
    public Rental(DateTimeOffset startDate, DateTimeOffset expectedReturnDate, decimal startKm) : this()
    {
        this.StartDate = startDate;
        this.ExpectedReturnDate = expectedReturnDate;
        this.StartKm = startKm;
    }

    public void CalculateBasePrice()
    {
        switch (this.SelectedPlanType)
        {
            case EPricingPlanType.Daily:
                this.BaseRentalPrice = this.PricingPlan.DailyPlan.DailyRate + this.PricingPlan.DailyPlan.PricePerKm;
                break;
            case EPricingPlanType.Controlled:
                this.BaseRentalPrice = this.PricingPlan.ControlledPlan.DailyRate + this.PricingPlan.ControlledPlan.PricePerKmExtrapolated;
                break;
            case EPricingPlanType.Free:
                this.BaseRentalPrice = this.PricingPlan.FreePlan.FixedRate;
                break;
        }
    }

    public void AddMultiplyRateService(List<RateService> rateServices) => this.RateServices.AddRange(rateServices);

    public void AddRentalService(RateService service) => this.RateServices.Add(service);

    public void RemoveRentalService(RateService service) => this.RateServices.Remove(service);

    public void SetEstimatedKilometers(decimal estimatedKilometers) => this.EstimatedKilometers = estimatedKilometers;

    public void SetStatus(ERentalStatus status) => this.Status = status;

    public void SetPricingPlanType(EPricingPlanType selectedPlanType) => this.SelectedPlanType = selectedPlanType;

    public void SetFinalPrice(decimal finalPrice) => this.FinalPrice = finalPrice;

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

    public void AssociatePricingPlan(PricingPlan PricingPlan)
    {
        this.PricingPlan = PricingPlan;
        this.PricingPlanId = PricingPlan.Id;
    }

    public override void Update(Rental updatedEntity)
    {
        this.StartDate = updatedEntity.StartDate;
        this.ExpectedReturnDate = updatedEntity.ExpectedReturnDate;
        this.StartKm = updatedEntity.StartKm;

        if (updatedEntity.Employee is not null)
        {
            this.Employee = updatedEntity.Employee;
            this.EmployeeId = updatedEntity.Employee.Id;
        }

        this.Client = updatedEntity.Client;
        this.ClientId = updatedEntity.Client.Id;
        this.Driver = updatedEntity.Driver;
        this.DriverId = updatedEntity.Driver.Id;
        this.PricingPlan = updatedEntity.PricingPlan;
        this.PricingPlanId = updatedEntity.PricingPlan.Id;
        this.RateServices = updatedEntity.RateServices;
    }
}

public enum ERentalStatus
{
    Open,
    Completed,
    Canceled
}

public enum EPricingPlanType
{
    Daily,
    Controlled,
    Free
}