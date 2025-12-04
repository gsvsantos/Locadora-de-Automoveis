using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Delete;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetById;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/billing-plan")]
[Authorize("AdminOrEmployeePolicy")]
public class BillingPlanController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateBillingPlanRequest request)
    {
        Result<CreateBillingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllBillingPlanRequest? request)
    {
        request = request ?? new GetAllBillingPlanRequest(null);

        Result<GetAllBillingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }
        return Ok(result.Value);
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdBillingPlanRequest request = new(id);

        Result<GetByIdBillingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBillingPlanRequestPartial partialRequest)
    {

        UpdateBillingPlanRequest request = mapper.Map<UpdateBillingPlanRequest>((partialRequest, id));

        Result<UpdateBillingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteBillingPlanRequest request = new(id);

        Result<DeleteBillingPlanResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}
