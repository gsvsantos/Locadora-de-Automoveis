using AutoMapper;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.Create;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetById;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.Update;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class RentalExtraProfile : Profile
{
    public RentalExtraProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllRentalExtraRequestPartial, GetAllRentalExtraRequest>()
            .ConvertUsing(src => new GetAllRentalExtraRequest(
                src.Quantity,
                src.IsActive
            ));

        // DTOs
        CreateMap<RentalExtra, RentalExtraDto>()
            .ConvertUsing(src => new RentalExtraDto(
                src.Id,
                src.Name,
                src.Price,
                src.IsDaily,
                src.Type.ToString(),
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<CreateRentalExtraRequest, RentalExtra>()
            .ConvertUsing(src => new RentalExtra(
                src.Name,
                src.Price
            ));

        // GetALl
        CreateMap<List<RentalExtra>, GetAllRentalExtraResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllRentalExtraResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<RentalExtraDto>(c)).ToImmutableList()
                    ?? ImmutableList<RentalExtraDto>.Empty
            ));

        // GetById
        CreateMap<RentalExtra, GetByIdRentalExtraResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdRentalExtraResponse(
               ctx.Mapper.Map<RentalExtraDto>(src)
            ));

        // Update
        CreateMap<UpdateRentalExtraRequest, RentalExtra>()
            .ConvertUsing(src => new RentalExtra(
                src.Name,
                src.Price
            )
            { Id = src.Id });
    }
}
