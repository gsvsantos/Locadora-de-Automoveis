namespace LocadoraDeAutomoveis.Domain.Shared;

public interface IRepository<T> where T : BaseEntity<T>
{
    Task AddAsync(T entity);

    Task<bool> UpdateAsync(Guid id, T updatedEntity);

    Task<bool> DeleteAsync(Guid id);

    Task<List<T>> GetAllAsync();

    Task<List<T>> GetAllAsync(int quantity);

    Task<List<T>> GetAllAsync(bool isActive);

    Task<List<T>> GetAllAsync(int quantity, bool isActive);

    Task<T?> GetByIdAsync(Guid id);
}
