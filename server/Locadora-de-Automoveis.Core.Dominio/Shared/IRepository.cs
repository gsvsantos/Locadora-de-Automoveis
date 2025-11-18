namespace Locadora_de_Automoveis.Core.Dominio.Shared;

public interface IRepository<T> where T : BaseEntity<T>
{
    Task AddAsync(T entity);

    Task<bool> UpdateAsync(Guid id, T updatedEntity);

    Task<bool> DeleteAsync(Guid id);

    Task<List<T>> GetAllAsync();

    Task<List<T>> GetAllAsync(int quantity);

    Task<T?> GetByIdAsync(Guid id);
}
