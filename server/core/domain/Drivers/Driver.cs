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
    public DateTimeOffset LicenseValidity { get; set; }
    public Guid ClientCPFId { get; set; }
    public Client ClientCPF { get; set; } = null!;
    public Guid? ClientCNPJId { get; set; }
    public Client? ClientCNPJ { get; set; } = null!;

    public Driver() { }
    public Driver(string fullName, string email, string phoneNumber,
        string document, string licenseNumber, DateTimeOffset licenseValidity
    ) : this()
    {
        this.FullName = fullName;
        this.Email = email;
        this.PhoneNumber = phoneNumber;
        this.Document = document;
        this.LicenseNumber = licenseNumber;
        this.LicenseValidity = licenseValidity;
    }

    public void AssociateClientCPF(Client client)
    {
        if (this.ClientCPF is not null && this.ClientCPF.Id.Equals(client.Id))
        {
            return;
        }

        if (this.ClientCPF is not null)
        {
            DisassociateClientCPF();
        }

        client.AssociateDriver(this);

        this.ClientCPF = client;
        this.ClientCPFId = client.Id;
    }

    public void DisassociateClientCPF()
    {
        if (this.ClientCPF is null)
        {
            return;
        }

        this.ClientCPF.DisassociateDriver();

        this.ClientCPF = null!;
        this.ClientCPFId = Guid.Empty;
    }

    public void AssociateClientCNPJ(Client client)
    {
        if (this.ClientCNPJ is not null && this.ClientCNPJ.Id.Equals(client.Id))
        {
            return;
        }

        if (this.ClientCNPJ is not null)
        {
            DisassociateClientCNPJ();
        }

        client.AssociateDriver(this);

        this.ClientCNPJ = client;
        this.ClientCNPJId = client.Id;
    }

    public void DisassociateClientCNPJ()
    {
        if (this.ClientCNPJ is null)
        {
            return;
        }

        DisassociateClientCPF();

        this.ClientCNPJ.DisassociateDriver();

        this.ClientCNPJ = null!;
        this.ClientCNPJId = Guid.Empty;
    }

    public override void Update(Driver updatedEntity)
    {
        this.FullName = updatedEntity.FullName;
        this.Email = updatedEntity.Email;
        this.PhoneNumber = updatedEntity.PhoneNumber;
        this.Document = updatedEntity.Document;
        this.LicenseNumber = updatedEntity.LicenseNumber;
        this.LicenseValidity = updatedEntity.LicenseValidity;
        this.ClientCPFId = updatedEntity.ClientCPFId;
        this.ClientCPF = updatedEntity.ClientCPF;
    }
}
