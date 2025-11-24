using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Vehicles;

public class VehicleRepository(AppDbContext context)
    : BaseRepository<Vehicle>(context), IRepositoryVehicle
{
    public override Task<List<Vehicle>> GetAllAsync() =>
        this.records.Include(g => g.Group).ToListAsync();

    public override Task<List<Vehicle>> GetAllAsync(int quantity) =>
        this.records.Include(g => g.Group).Take(quantity).ToListAsync();

    public override Task<Vehicle?> GetByIdAsync(Guid entityId) =>
        this.records.Include(g => g.Group)
        .FirstOrDefaultAsync(d => d.Id.Equals(entityId));
}
