using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Clients.Commands.Create;
using LocadoraDeAutomoveis.Application.Clients.Commands.Delete;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetById;
using LocadoraDeAutomoveis.Application.Clients.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/client")]
[Authorize("AdminOrEmployeePolicy")]
public class ClientController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        Result<CreateClientResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllClientRequest? request)
    {
        request = request ?? new GetAllClientRequest(null);

        Result<GetAllClientResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }
        return Ok(result.Value);
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdClientRequest request = new(id);

        Result<GetByIdClientResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientRequestPartial partialRequest)
    {
        UpdateClientRequest request = mapper.Map<UpdateClientRequest>((partialRequest, id));

        Result<UpdateClientResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteClientRequest request = new(id);

        Result<DeleteClientResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}

