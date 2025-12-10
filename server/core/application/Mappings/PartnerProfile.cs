using AutoMapper;
using LocadoraDeAutomoveis.Application.Partners.Commands.Create;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using LocadoraDeAutomoveis.Application.Partners.Commands.Update;
using LocadoraDeAutomoveis.Domain.Partners;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class PartnerProfile : Profile
{
    public PartnerProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllPartnerRequestPartial, GetAllPartnerRequest>()
            .ConvertUsing(src => new GetAllPartnerRequest(
                src.Quantity,
                src.IsActive
            ));

        // Update
        CreateMap<(UpdatePartnerRequestPartial p, Guid id), UpdatePartnerRequest>()
            .ConvertUsing(src => new UpdatePartnerRequest(
                src.id,
                src.p.FullName
            ));

        // DTOs
        CreateMap<Partner, PartnerDto>()
            .ConvertUsing(src => new PartnerDto(
                src.Id,
                src.FullName,
                src.IsActive
            ));

        CreateMap<Partner, GetAllPartnerDto>()
            .ConvertUsing(src => new GetAllPartnerDto(
                src.Id,
                src.FullName,
                src.IsActive,
                src.Coupons.Count
            ));

        CreateMap<Partner, GetCouponsPartnerDto>()
            .ConvertUsing((src, dest, ctx) => new GetCouponsPartnerDto(
                src.Id,
                src.FullName,
                src.IsActive,
                src.Coupons.Select(c => ctx.Mapper.Map<CouponDto>(c)).ToImmutableList()
                ?? ImmutableList<CouponDto>.Empty
            ));

        // HANDLERS
        // Create
        CreateMap<CreatePartnerRequest, Partner>()
            .ConvertUsing(src => new Partner(
                src.FullName
            ));

        // GetALl
        CreateMap<List<Partner>, GetAllPartnerResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllPartnerResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<GetAllPartnerDto>(c)).ToImmutableList()
                    ?? ImmutableList<GetAllPartnerDto>.Empty
            ));

        // GetCoupons
        CreateMap<Partner, GetCouponsPartnerResponse>()
            .ConvertUsing((src, dest, ctx) => new GetCouponsPartnerResponse(
               ctx.Mapper.Map<GetCouponsPartnerDto>(src)
            ));

        // Update
        CreateMap<UpdatePartnerRequest, Partner>()
            .ConvertUsing(src => new Partner(
                src.FullName
            )
            { Id = src.Id });
    }
}
