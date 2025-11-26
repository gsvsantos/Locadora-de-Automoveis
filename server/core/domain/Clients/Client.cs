using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Clients;

public class Client : BaseEntity<Client>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public EClientType ClientType { get; set; }
    public string Document { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; } = null;
    public DateTimeOffset? LicenseExpiry { get; set; }
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; } = 0;

    public Client() { }
    public Client(string fullName, string email, string phoneNumber, string state, string city,
        string neighborhood, string street, int number, string document
    ) : this()
    {
        this.FullName = fullName;
        this.Email = email;
        this.PhoneNumber = phoneNumber;
        this.State = state;
        this.City = city;
        this.Neighborhood = neighborhood;
        this.Street = street;
        this.Number = number;
        this.Document = document;
    }

    public void MarkAsJuridical() => this.ClientType = EClientType.Juristic;

    public void MarkAsPhysical() => this.ClientType = EClientType.Physical;

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

public enum EClientType
{
    Physical,
    Juristic
}
