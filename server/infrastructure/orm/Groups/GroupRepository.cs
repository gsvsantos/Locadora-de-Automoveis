using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Infrastructure.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.Groups;

public class GroupRepository(AppDbContext context)
    : BaseRepository<Group>(context), IRepositoryGroup;
