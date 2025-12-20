using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Delete;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAvailableById;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetById;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/vehicle")]
public class VehicleController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Create([FromForm] CreateVehicleRequest request)
    {
        Result<CreateVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllVehicleRequestPartial partialRequest)
    {
        GetAllVehicleRequest request = mapper.Map<GetAllVehicleRequest>(partialRequest);

        Result<GetAllVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdVehicleRequest request = new(id);

        Result<GetByIdVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateVehicleRequestPartial partialRequest)
    {

        UpdateVehicleRequest request = mapper.Map<UpdateVehicleRequest>((partialRequest, id));

        Result<UpdateVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteVehicleRequest request = new(id);

        Result<DeleteVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("available")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAvailable([FromQuery] GetAllAvailableVehiclesRequest request)
    {
        Result<PagedResult<VehicleDto>> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        return Ok(result.Value);
    }

    [HttpGet("available/{id:guid}")]
    [Authorize("EveryonePolicy")]
    public async Task<IActionResult> GetVehicleForRental(Guid id)
    {
        GetAvailableByIdVehicleRequest request = new(id);

        Result<GetAvailableByIdVehicleResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}
