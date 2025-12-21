using AutoMapper;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetById;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Update;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class BillingPlanProfile : Profile
{
    public BillingPlanProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllBillingPlanRequestPartial, GetAllBillingPlanRequest>()
            .ConvertUsing(src => new GetAllBillingPlanRequest(
                src.Quantity,
                src.IsActive
            ));

        // Update
        CreateMap<(UpdateBillingPlanRequestPartial p, Guid id), UpdateBillingPlanRequest>()
            .ConvertUsing(src => new UpdateBillingPlanRequest(
                src.id,
                src.p.GroupId,
                new(src.p.DailyBilling.DailyRate, src.p.DailyBilling.PricePerKm),
                new(src.p.ControlledBilling.DailyRate, src.p.ControlledBilling.PricePerKmExtrapolated),
                new(src.p.FreeBilling.FixedRate)
            ));

        // DTOs
        CreateMap<Group, BillingPlanGroupDto>()
            .ConvertUsing(src => new BillingPlanGroupDto(
                src.Id,
                src.Name
            ));

        CreateMap<BillingPlan, BillingPlanDto>()
            .ConvertUsing((src, dest, ctx) => new BillingPlanDto(
                src.Id,
                ctx.Mapper.Map<BillingPlanGroupDto>(src.Group),
                new(src.Daily.DailyRate, src.Daily.PricePerKm),
                new(src.Controlled.DailyRate, src.Controlled.PricePerKmExtrapolated),
                new(src.Free.FixedRate),
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<(CreateBillingPlanRequest r, Group g), BillingPlan>()
            .ConvertUsing(src => new BillingPlan(
                $"{src.g.Name} - Billing Plans",
                src.r.DailyBilling.ToProps(),
                src.r.ControlledBilling.ToProps(),
                src.r.FreeBilling.ToProps()
            ));

        // GetAll
        CreateMap<List<BillingPlan>, GetAllBillingPlanResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllBillingPlanResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<BillingPlanDto>(c)).ToImmutableList()
                    ?? ImmutableList<BillingPlanDto>.Empty
            ));

        // GetById
        CreateMap<BillingPlan, GetByIdBillingPlanResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdBillingPlanResponse(
               ctx.Mapper.Map<BillingPlanDto>(src)
            ));

        // Update
        CreateMap<(UpdateBillingPlanRequest r, Group g), BillingPlan>()
            .ConvertUsing(src => new BillingPlan(
                $"{src.g.Name} - Billing Plans",
                src.r.DailyBilling.ToProps(),
                src.r.ControlledBilling.ToProps(),
                src.r.FreeBilling.ToProps()
            )
            { Id = src.r.Id });
    }
}
