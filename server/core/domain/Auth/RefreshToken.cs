using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Auth;

public class RefreshToken : BaseEntity<RefreshToken>
{
    public Guid UserAuthenticatedId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedDateUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpirationDateUtc { get; set; }
    public DateTimeOffset? RevokedDateUtc { get; set; }
    public string? RevocationReason { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? CreationIp { get; set; }
    public string? UserAgent { get; set; }

    public bool IsValid
    {
        get
        {
            return this.RevokedDateUtc is null && DateTimeOffset.UtcNow <= this.ExpirationDateUtc;
        }
    }

    public override void AssociateUser(User? user)
    {
        base.AssociateUser(user);
        this.UserAuthenticatedId = user!.Id;
    }

    public override void Update(RefreshToken updatedEntity)
    {
        // update isnt necessary =)
        // override only for the others methods and props from baseentity =]
    }
}