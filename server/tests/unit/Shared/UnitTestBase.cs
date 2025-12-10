using AutoMapper;
using LocadoraDeAutomoveis.Application.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Shared;
public abstract class UnitTestBase
{
    protected readonly IMapper mapper;

    protected UnitTestBase()
    {
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(ApplicationAssemblyReference).Assembly);
        });

        this.mapper = config.CreateMapper();
    }
}
