using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Clients;

public class Client : BaseEntity<Client>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Document { get; set; }
    public Address? Address { get; set; }
    public EClientType Type { get; set; }
    public string? LicenseNumber { get; set; } = null;
    public DateTimeOffset? LicenseValidity { get; set; }
    public Guid? JuristicClientId { get; set; }
    public Client? JuristicClient { get; set; }
    public Guid? LoginUserId { get; set; }
    public User? LoginUser { get; set; }
    public string PreferredLanguage { get; private set; } = "pt-BR";

    public Client() { }

    public Client(string fullName, string email, string phoneNumber) : this()
    {
        this.FullName = fullName;
        this.Email = email;
        this.PhoneNumber = phoneNumber;
    }
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

    public void DefineType(EClientType type)
    {
        this.Type = type;
    }

    public void SetLicenseNumber(string licenseNumber)
    {
        this.LicenseNumber = licenseNumber;
    }

    public void SetLicenseValidity(DateTimeOffset licenseValidity)
    {
        this.LicenseValidity = licenseValidity;
    }

    public void SetLicense(string licenseNumber, DateTimeOffset licenseValidity)
    {
        this.LicenseNumber = licenseNumber;
        this.LicenseValidity = licenseValidity;
    }

    public void SetPreferredLanguage(string language)
    {
        this.PreferredLanguage = language;
    }

    public void AssociateJuristicClient(Client juristiClient)
    {
        this.JuristicClientId = juristiClient.Id;
        this.JuristicClient = juristiClient;
    }

    public void AssociateLoginUser(User user)
    {
        this.LoginUser = user;
        this.LoginUserId = user.Id;
    }

    public bool HasFullProfile()
    {
        return this.Address is not null &&
             !string.IsNullOrWhiteSpace(this.Document) &&
             !string.IsNullOrWhiteSpace(this.PhoneNumber);
    }

    public void CompleteProfile(string document, Address address, string? licenseNumber = null, DateTimeOffset? licenseValidity = null)
    {
        this.Document = document;
        this.Address = address;

        if (licenseNumber != null)
        {
            this.LicenseNumber = licenseNumber;
        }

        if (licenseValidity != null)
        {
            this.LicenseValidity = licenseValidity;
        }
    }

    public static Client CreateTenantCopyFromGlobal(Client globalClient, Guid tenantId, User loginUser, User createdByUser)
    {
        Client tenantClient = new(
        globalClient.FullName,
        globalClient.Email,
        globalClient.PhoneNumber
    );

        tenantClient.DefineType(globalClient.Type);

        if (!string.IsNullOrWhiteSpace(globalClient.Document) && globalClient.Address is not null)
        {
            Address newAddress = new(
                 globalClient.Address.State,
                 globalClient.Address.City,
                 globalClient.Address.Neighborhood,
                 globalClient.Address.Street,
                 globalClient.Address.Number
             );

            tenantClient.CompleteProfile(
                globalClient.Document,
                newAddress,
                globalClient.LicenseNumber,
                globalClient.LicenseValidity
            );
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(globalClient.LicenseNumber))
            {
                tenantClient.SetLicenseNumber(globalClient.LicenseNumber);
            }

            if (globalClient.LicenseValidity.HasValue)
            {
                tenantClient.SetLicenseValidity(globalClient.LicenseValidity.Value);
            }
        }

        tenantClient.AssociateTenant(tenantId);
        tenantClient.AssociateLoginUser(loginUser);
        tenantClient.AssociateUser(createdByUser);

        return tenantClient;
    }

    public override void Update(Client updatedEntity)
    {
        this.FullName = updatedEntity.FullName;
        this.Email = updatedEntity.Email;
        this.PhoneNumber = updatedEntity.PhoneNumber;
        this.Document = updatedEntity.Document;
        this.Address = updatedEntity.Address;
        this.LicenseNumber = updatedEntity.LicenseNumber;
        this.LicenseValidity = updatedEntity.LicenseValidity;
        DefineType(updatedEntity.Type);
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