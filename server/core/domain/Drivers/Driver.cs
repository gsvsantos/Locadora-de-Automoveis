using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Drivers;

public class Driver : BaseEntity<Driver>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseValidity { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public Driver() { }
    public Driver(string fullName, string email, string phoneNumber,
        string document, string licenseNumber, string licenseValidity,
        Guid clientId, Client client
    ) : this()
    {
        this.FullName = fullName;
        this.Email = email;
        this.PhoneNumber = phoneNumber;
        this.Document = document;
        this.LicenseNumber = licenseNumber;
        this.LicenseValidity = licenseValidity;
        this.ClientId = clientId;
        this.Client = client;
    }

    public void AssociateClient(Client client)
    {
        if (this.Client is not null && this.Client.Id.Equals(client.Id))
        {
            return;
        }

        if (this.Client is not null)
        {
            DisassociateClient();
        }

        this.Client = client;
        this.ClientId = client.Id;
    }

    public void DisassociateClient()
    {
        if (this.Client is null)
        {
            return;
        }

        this.Client = null!;
        this.ClientId = Guid.Empty;
    }

    public override void Update(Driver updatedEntity)
    {
        this.FullName = updatedEntity.FullName;
        this.Email = updatedEntity.Email;
        this.PhoneNumber = updatedEntity.PhoneNumber;
        this.Document = updatedEntity.Document;
        this.LicenseNumber = updatedEntity.LicenseNumber;
        this.LicenseValidity = updatedEntity.LicenseValidity;
        this.ClientId = updatedEntity.ClientId;
        this.Client = updatedEntity.Client;
    }
}
