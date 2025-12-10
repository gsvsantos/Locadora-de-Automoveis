using AutoMapper;
using LocadoraDeAutomoveis.Application.Configurations.Commands.Configure;
using LocadoraDeAutomoveis.Application.Configurations.Commands.Details;
using LocadoraDeAutomoveis.Domain.Configurations;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class ConfigurationProfile : Profile
{
    public ConfigurationProfile()
    {
        // DTOs
        CreateMap<Configuration, ConfigurationDto>()
            .ConvertUsing(src => new ConfigurationDto(
                src.Id,
                src.GasolinePrice,
                src.GasPrice,
                src.DieselPrice,
                src.AlcoholPrice
            ));

        // HANDLERS
        // Configure
        CreateMap<ConfigureRequest, Configuration>()
            .ConvertUsing(src => new Configuration(
                src.GasolinePrice,
                src.GasPrice,
                src.DieselPrice,
                src.AlcoholPrice
            ));

        // Details
        CreateMap<Configuration, DetailsResponse>()
            .ConvertUsing((src, dest, ctx) => new DetailsResponse(
               ctx.Mapper.Map<ConfigurationDto>(src)
            ));
    }
}
