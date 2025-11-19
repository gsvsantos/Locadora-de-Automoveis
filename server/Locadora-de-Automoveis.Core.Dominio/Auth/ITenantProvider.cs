namespace Locadora_de_Automoveis.Core.Dominio.Auth;

public interface ITenantProvider
{
    Guid? UserId { get; }
}