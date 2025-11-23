using FluentResults;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Create;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Delete;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetById;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/pricing-plan")]
[Authorize("AdminOrEmployeePolicy")]
public class PricingPlanController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePricingPlanRequest request)
    {
        Result<CreatePricingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllPricingPlanRequest? request)
    {
        request = request ?? new GetAllPricingPlanRequest(null);

        Result<GetAllPricingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }
        return Ok(result.Value);
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdPricingPlanRequest request = new(id);

        Result<GetByIdPricingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePricingPlanRequestPartial partialRequest)
    {

        UpdatePricingPlanRequest request = new(
            id,
            partialRequest.GroupId,
            new(partialRequest.DailyPlan.DailyRate, partialRequest.DailyPlan.PricePerKm),
            new(partialRequest.ControlledPlan.DailyRate, partialRequest.ControlledPlan.AvailableKm, partialRequest.ControlledPlan.PricePerKmExtrapolated),
            new(partialRequest.FreePlan.FixedRate)
        );

        Result<UpdatePricingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeletePricingPlanRequest request = new(id);

        Result<DeletePricingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}
