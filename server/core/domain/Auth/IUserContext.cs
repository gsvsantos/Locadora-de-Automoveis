namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IUserContext
{
    Guid? UserId { get; }
    Guid GetUserId() => this.UserId.GetValueOrDefault();
    bool IsInRole(string roleName);
}
