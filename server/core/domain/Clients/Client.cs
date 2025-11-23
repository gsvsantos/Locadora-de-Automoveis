using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Clients;

public class Client : BaseEntity<Client>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsJuridical { get; set; } = false;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; } = 0;
    public string? Document { get; set; } = null;
    public string? LicenseNumber { get; set; } = null;
    public Guid DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    public Client() { }
    public Client(string name, string email, string phoneNumber, string state,
        string city, string neighborhood, string street, int number,
        string? document, string? cnh
    ) : this()
    {
        this.FullName = name;
        this.Email = email;
        this.PhoneNumber = phoneNumber;
        this.State = state;
        this.City = city;
        this.Neighborhood = neighborhood;
        this.Street = street;
        this.Number = number;
        this.Document = document;
        this.LicenseNumber = cnh;
    }

    public void MarkAsJuridical() => this.IsJuridical = true;

    public void MarkAsPhysical() => this.IsJuridical = false;

    public void AssociateClient(Driver driver)
    {
        if (this.Driver is not null && this.Driver.Id.Equals(driver.Id))
        {
            return;
        }

        if (this.Driver is not null)
        {
            DisassociateDriver();
        }

        this.Driver = driver;
        this.DriverId = driver.Id;
    }

    public void DisassociateDriver()
    {
        if (this.Driver is null)
        {
            return;
        }

        this.Driver = null!;
        this.DriverId = Guid.Empty;
    }

    public override void Update(Client updatedEntity)
    {
        this.FullName = updatedEntity.FullName;
        this.Email = updatedEntity.Email;
        this.PhoneNumber = updatedEntity.PhoneNumber;
        this.State = updatedEntity.State;
        this.City = updatedEntity.City;
        this.Neighborhood = updatedEntity.Neighborhood;
        this.Street = updatedEntity.Street;
        this.Number = updatedEntity.Number;
        this.Document = updatedEntity.Document;
        this.LicenseNumber = updatedEntity.LicenseNumber;
    }
}
