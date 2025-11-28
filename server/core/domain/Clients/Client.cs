using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Clients;

public class Client : BaseEntity<Client>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public Address Address { get; set; } = null!;
    public EClientType ClientType { get; set; }
    public string? LicenseNumber { get; set; } = null;
    public DateTimeOffset? LicenseExpiry { get; set; }
    public Guid? JuristicClientId { get; set; }
    public Client? JuristicClient { get; set; }

    public Client() { }
    public Client(string fullName, string email, string phoneNumber,
        string document, Address address
    ) : this()
    {
        this.FullName = fullName;
        this.Email = email;
        this.PhoneNumber = phoneNumber;
        this.Document = document;
        this.Address = address;
    }

    public void SetClientType(EClientType clientType) => this.ClientType = clientType;

    public void SetLicenseNumber(string licenseNumber) => this.LicenseNumber = licenseNumber;

    public void AssociateJuristicClient(Client juristiClient)
    {
        this.JuristicClientId = juristiClient.Id;
        this.JuristicClient = juristiClient;
    }

    public override void Update(Client updatedEntity)
    {
        this.FullName = updatedEntity.FullName;
        this.Email = updatedEntity.Email;
        this.PhoneNumber = updatedEntity.PhoneNumber;
        this.Document = updatedEntity.Document;
        this.Address = updatedEntity.Address;
        this.LicenseNumber = updatedEntity.LicenseNumber;
    }
}

public record Address(
    string State,
    string City,
    string Neighborhood,
    string Street,
    int Number
);

public enum EClientType
{
    Individual,
    Business
}
