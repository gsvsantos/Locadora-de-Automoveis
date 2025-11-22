using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Groups;

public class GroupRepository(AppDbContext context)
    : BaseRepository<Group>(context), IRepositoryGroup
{
    public override Task<List<Group>> GetAllAsync() =>
        this.records.Include(g => g.Vehicles).ToListAsync();

    public override Task<List<Group>> GetAllAsync(int quantity) =>
        this.records.Include(g => g.Vehicles).Take(quantity).ToListAsync();

    public override Task<Group?> GetByIdAsync(Guid entityId) =>
        this.records.Include(g => g.Vehicles).FirstOrDefaultAsync(g => g.Id.Equals(entityId));
}
