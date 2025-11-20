using LocadoraDeAutomoveis.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Shared;

public class BaseRepository<T>(AppDbContext context) where T : BaseEntity<T>
{
    protected readonly DbSet<T> records = context.Set<T>();

    public async Task AddAsync(T newEntity) => await this.records.AddAsync(newEntity);

    public async Task AddMultiplyAsync(IList<T> entities) => await this.records.AddRangeAsync(entities);

    public async Task<bool> UpdateAsync(Guid entityId, T updatedEntity)
    {
        T? existingEntity = await GetByIdAsync(entityId);

        if (existingEntity is null)
        {
            return false;
        }

        existingEntity.Update(updatedEntity);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid entityId)
    {
        T? existingEntity = await GetByIdAsync(entityId);

        if (existingEntity is null)
        {
            return false;
        }

        this.records.Remove(existingEntity);

        return true;
    }

    public virtual async Task<List<T>> GetAllAsync() => await this.records.ToListAsync();

    public virtual async Task<List<T>> GetAllAsync(int quantity) => await this.records.Take(quantity).ToListAsync();

    public virtual async Task<T?> GetByIdAsync(Guid entityId) => await this.records.FirstOrDefaultAsync(x => x.Id.Equals(entityId));
}
