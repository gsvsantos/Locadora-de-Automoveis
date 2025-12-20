using LocadoraDeAutomoveis.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Shared;

public class BaseRepository<T>(AppDbContext context) where T : BaseEntity<T>
{
    protected readonly DbSet<T> records = context.Set<T>();

    public async Task AddAsync(T newEntity)
    {
        await this.records.AddAsync(newEntity);
    }

    public async Task AddMultiplyAsync(IList<T> entities)
    {
        await this.records.AddRangeAsync(entities);
    }

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

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await this.records.ToListAsync();
    }

    public virtual async Task<List<T>> GetAllAsync(int quantity)
    {
        return await this.records.Take(quantity).ToListAsync();
    }

    public virtual async Task<List<T>> GetAllAsync(bool isActive)
    {
        return await this.records.Where(x => x.IsActive == isActive).ToListAsync();
    }

    public virtual async Task<List<T>> GetAllAsync(int quantity, bool isActive)
    {
        return await this.records.Where(x => x.IsActive == isActive).Take(quantity).ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid entityId)
    {
        return await this.records.FirstOrDefaultAsync(x => x.Id.Equals(entityId));
    }
}
