using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Drivers;

public interface IRepositoryDriver : IRepository<Driver>
{
    Task<bool> HasDriversByClient(Guid clientId);
}
