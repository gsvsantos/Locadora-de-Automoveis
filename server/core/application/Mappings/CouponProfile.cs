using AutoMapper;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Create;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllAvailable;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetById;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Update;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using LocadoraDeAutomoveis.Domain.Coupons;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class CouponProfile : Profile
{
    public CouponProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllCouponRequestPartial, GetAllCouponRequest>()
            .ConvertUsing(src => new GetAllCouponRequest(
                src.Quantity,
                src.IsActive
            ));

        // Update
        CreateMap<(UpdateCouponRequestPartial p, Guid id), UpdateCouponRequest>()
            .ConvertUsing(src => new UpdateCouponRequest(
                src.id,
                src.p.Name,
                src.p.DiscountValue,
                src.p.ExpirationDate,
                src.p.PartnerId
            ));

        // DTOs
        CreateMap<Coupon, CouponDto>()
            .ConvertUsing((src, dest, ctx) => new CouponDto(
                src.Id,
                src.Name,
                ctx.Mapper.Map<PartnerDto>(src.Partner),
                src.DiscountValue,
                src.ExpirationDate,
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<CreateCouponRequest, Coupon>()
            .ConvertUsing(src => new Coupon(
                src.Name,
                src.DiscountValue,
                src.ExpirationDate
            ));

        // GetAll
        CreateMap<List<Coupon>, GetAllCouponResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllCouponResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<CouponDto>(c)).ToImmutableList()
                    ?? ImmutableList<CouponDto>.Empty
            ));

        // GetById
        CreateMap<Coupon, GetByIdCouponResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdCouponResponse(
               ctx.Mapper.Map<CouponDto>(src)
            ));

        // Update
        CreateMap<UpdateCouponRequest, Coupon>()
            .ConvertUsing(src => new Coupon(
                src.Name,
                src.DiscountValue,
                src.ExpirationDate
            )
            { Id = src.Id });

        // GetAllAvailable
        CreateMap<List<Coupon>, GetAllAvailableCouponResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllAvailableCouponResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<CouponDto>(c)).ToImmutableList()
                    ?? ImmutableList<CouponDto>.Empty
            ));
    }
}
