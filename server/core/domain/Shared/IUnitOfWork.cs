namespace LocadoraDeAutomoveis.Core.Domain.Shared;

public interface IUnitOfWork
{
    Task CommitAsync();
    Task RollbackAsync();
}
