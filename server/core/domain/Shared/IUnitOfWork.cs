namespace LocadoraDeAutomoveis.Domain.Shared;

public interface IUnitOfWork
{
    Task CommitAsync();
    Task RollbackAsync();
}
