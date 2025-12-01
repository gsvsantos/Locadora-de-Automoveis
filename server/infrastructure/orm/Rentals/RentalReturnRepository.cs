using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Infrastructure.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.Rentals;

public class RentalReturnRepository(AppDbContext context)
    : BaseRepository<RentalReturn>(context), IRepositoryRentalReturn;
