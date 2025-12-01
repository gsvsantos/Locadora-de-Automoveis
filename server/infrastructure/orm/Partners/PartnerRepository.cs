using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Infrastructure.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.Partners;

public class PartnerRepository(AppDbContext context)
    : BaseRepository<Partner>(context), IRepositoryPartner;
