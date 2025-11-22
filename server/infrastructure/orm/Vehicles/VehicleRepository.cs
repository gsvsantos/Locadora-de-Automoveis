using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Infrastructure.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.Vehicles;

public class VehicleRepository(AppDbContext context)
    : BaseRepository<Vehicle>(context), IRepositoryVehicle;
