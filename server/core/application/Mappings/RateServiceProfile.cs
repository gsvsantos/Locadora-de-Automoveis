using AutoMapper;
using LocadoraDeAutomoveis.Application.RateServices.Commands.Create;
using LocadoraDeAutomoveis.Application.RateServices.Commands.GetAll;
using LocadoraDeAutomoveis.Application.RateServices.Commands.GetById;
using LocadoraDeAutomoveis.Application.RateServices.Commands.Update;
using LocadoraDeAutomoveis.Domain.RateServices;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class RateServiceProfile : Profile
{
    public RateServiceProfile()
    {
        // DTOs
        CreateMap<RateService, RateServiceDto>()
            .ConvertUsing(src => new RateServiceDto(
                src.Id,
                src.Name,
                src.Price,
                src.IsChargedPerDay,
                src.RateType.ToString()
            ));

        // HANDLERS
        // Create
        CreateMap<CreateRateServiceRequest, RateService>()
            .ConvertUsing(src => new RateService(
                src.Name,
                src.Price
            ));

        // GetALl
        CreateMap<List<RateService>, GetAllRateServiceResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllRateServiceResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<RateServiceDto>(c)).ToImmutableList()
                    ?? ImmutableList<RateServiceDto>.Empty
            ));

        // GetById
        CreateMap<RateService, GetByIdRateServiceResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdRateServiceResponse(
               ctx.Mapper.Map<RateServiceDto>(src)
            ));

        // Update
        CreateMap<UpdateRateServiceRequest, RateService>()
            .ConvertUsing(src => new RateService(
                src.Name,
                src.Price
            )
            { Id = src.Id });
    }
}
