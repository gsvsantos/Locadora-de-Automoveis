using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Delete;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetById;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/vehicle")]
[Authorize("AdminOrEmployeePolicy")]
public class VehicleController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CreateVehicleRequest request)
    {
        Result<CreateVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllVehicleRequestPartial partialRequest)
    {
        GetAllVehicleRequest request = mapper.Map<GetAllVehicleRequest>(partialRequest);

        Result<GetAllVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdVehicleRequest request = new(id);

        Result<GetByIdVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateVehicleRequestPartial partialRequest)
    {

        UpdateVehicleRequest request = mapper.Map<UpdateVehicleRequest>((partialRequest, id));

        Result<UpdateVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteVehicleRequest request = new(id);

        Result<DeleteVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}
