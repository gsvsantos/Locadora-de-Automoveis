using AutoMapper;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Create;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetById;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Update;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class PricingPlanProfile : Profile
{
    public PricingPlanProfile()
    {
        // CONTROLLER
        // Update
        CreateMap<(UpdatePricingPlanRequestPartial p, Guid id), UpdatePricingPlanRequest>()
            .ConvertUsing(src => new UpdatePricingPlanRequest(
                src.id,
                src.p.GroupId,
                new(src.p.DailyPlan.DailyRate, src.p.DailyPlan.PricePerKm),
                new(src.p.ControlledPlan.DailyRate, src.p.ControlledPlan.PricePerKmExtrapolated),
                new(src.p.FreePlan.FixedRate)
            ));

        // DTOs
        CreateMap<PricingPlan, PricingPlanDto>()
            .ConvertUsing(src => new PricingPlanDto(
                src.Id,
                $"{src.Group.Name} - Pricing Plans",
                new(src.DailyPlan.DailyRate, src.DailyPlan.PricePerKm),
                new(src.ControlledPlan.DailyRate, src.ControlledPlan.PricePerKmExtrapolated),
                new(src.FreePlan.FixedRate),
                src.GroupId
            ));

        // HANDLERS
        // Create
        CreateMap<(CreatePricingPlanRequest r, Group g), PricingPlan>()
            .ConvertUsing(src => new PricingPlan(
                $"{src.g.Name} - Pricing Plans",
                src.r.DailyPlan.ToProps(),
                src.r.ControlledPlan.ToProps(),
                src.r.FreePlan.ToProps()
            ));

        // GetALl
        CreateMap<List<PricingPlan>, GetAllPricingPlanResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllPricingPlanResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<PricingPlanDto>(c)).ToImmutableList()
                    ?? ImmutableList<PricingPlanDto>.Empty
            ));

        // GetById
        CreateMap<PricingPlan, GetByIdPricingPlanResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdPricingPlanResponse(
               ctx.Mapper.Map<PricingPlanDto>(src)
            ));

        // Update
        CreateMap<(UpdatePricingPlanRequest r, Group g), PricingPlan>()
            .ConvertUsing(src => new PricingPlan(
                $"{src.g.Name} - Pricing Plans",
                src.r.DailyPlan.ToProps(),
                src.r.ControlledPlan.ToProps(),
                src.r.FreePlan.ToProps()
            )
            { Id = src.r.Id });
    }
}
