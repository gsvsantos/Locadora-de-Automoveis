using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Clients.Commands.Create;
using LocadoraDeAutomoveis.Application.Clients.Commands.Delete;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetById;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetIndividuals;
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

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllClientRequestPartial partialRequest)
    {
        GetAllClientRequest request = mapper.Map<GetAllClientRequest>(partialRequest);

        Result<GetAllClientResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdClientRequest request = new(id);

        Result<GetByIdClientResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("business/{id:guid}/individuals")]
    public async Task<IActionResult> GetIndividualClients(Guid id)
    {
        GetIndividualsRequest request = new(id);

        Result<GetIndividualsResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientRequestPartial partialRequest)
    {
        UpdateClientRequest request = mapper.Map<UpdateClientRequest>((partialRequest, id));

        Result<UpdateClientResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteClientRequest request = new(id);

        Result<DeleteClientResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}

