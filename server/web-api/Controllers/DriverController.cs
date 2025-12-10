using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Drivers.Commands.Create;
using LocadoraDeAutomoveis.Application.Drivers.Commands.Delete;
using LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Drivers.Commands.GetById;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/driver")]
[Authorize("AdminOrEmployeePolicy")]
public class DriverController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDriverRequest request)
    {
        Result<CreateDriverResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllDriverRequestPartial partialRequest)
    {
        GetAllDriverRequest request = mapper.Map<GetAllDriverRequest>(partialRequest);

        Result<GetAllDriverResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdDriverRequest request = new(id);

        Result<GetByIdDriverResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteDriverRequest request = new(id);

        Result<DeleteDriverResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}
