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
        // Update
        CreateMap<(UpdateBillingPlanRequestPartial p, Guid id), UpdateBillingPlanRequest>()
            .ConvertUsing(src => new UpdateBillingPlanRequest(
                src.id,
                src.p.GroupId,
                new(src.p.DailyPlan.DailyRate, src.p.DailyPlan.PricePerKm),
                new(src.p.ControlledPlan.DailyRate, src.p.ControlledPlan.PricePerKmExtrapolated),
                new(src.p.FreePlan.FixedRate)
            ));

        // DTOs
        CreateMap<BillingPlan, BillingPlanDto>()
            .ConvertUsing(src => new BillingPlanDto(
                src.Id,
                $"{src.Group.Name} - Billing Plans",
                new(src.Daily.DailyRate, src.Daily.PricePerKm),
                new(src.Controlled.DailyRate, src.Controlled.PricePerKmExtrapolated),
                new(src.Free.FixedRate),
                src.GroupId
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

        // GetALl
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
                src.r.DailyPlan.ToProps(),
                src.r.ControlledPlan.ToProps(),
                src.r.FreePlan.ToProps()
            )
            { Id = src.r.Id });
    }
}
