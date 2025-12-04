using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.Create;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.Delete;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetById;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/rental-extras")]
[Authorize("AdminOrEmployeePolicy")]
public class RentalExtraController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRentalExtraRequest request)
    {
        Result<CreateRentalExtraResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRentalExtraRequest? request)
    {
        request = request ?? new GetAllRentalExtraRequest(null);

        Result<GetAllRentalExtraResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }
        return Ok(result.Value);
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdRentalExtraRequest request = new(id);

        Result<GetByIdRentalExtraResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRentalExtraRequestPartial partialRequest)
    {

        UpdateRentalExtraRequest request = mapper.Map<UpdateRentalExtraRequest>((partialRequest, id));

        Result<UpdateRentalExtraResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteRentalExtraRequest request = new(id);

        Result<DeleteRentalExtraResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}
