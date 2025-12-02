namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IUserContext
{
    Guid? UserId { get; }
    Guid GetUserId()
    {
        return this.UserId.GetValueOrDefault();
    }

    bool IsInRole(string roleName);
}
