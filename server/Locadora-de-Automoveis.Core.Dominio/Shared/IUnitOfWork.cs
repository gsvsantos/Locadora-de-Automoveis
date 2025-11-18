namespace Locadora_de_Automoveis.Core.Dominio.Shared;

public interface IUnitOfWork
{
    Task CommitAsync();
    Task RollbackAsync();
}
