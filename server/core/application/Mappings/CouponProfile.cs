using AutoMapper;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Create;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Update;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using LocadoraDeAutomoveis.Domain.Coupons;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class CouponProfile : Profile
{
    public CouponProfile()
    {
        // CONTROLLER
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
            .ConvertUsing(src => new CouponDto(
                src.Id,
                src.Name,
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

        // Update
        CreateMap<UpdateCouponRequest, Coupon>()
            .ConvertUsing(src => new Coupon(
                src.Name,
                src.DiscountValue,
                src.ExpirationDate
            ));
    }
}
