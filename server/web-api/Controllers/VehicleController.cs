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
public class VehicleController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
    {
        Result<CreateVehicleResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] int? quantity, [FromQuery] Guid? groupId)
    {
        GetAllVehicleRequest request = new(quantity, groupId);

        Result<GetAllVehicleResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }
        return Ok(result.Value);
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdVehicleRequest request = new(id);

        Result<GetByIdVehicleResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleRequestPartial partialRequest)
    {

        UpdateVehicleRequest request = new(
            id,
            partialRequest.LicensePlate,
            partialRequest.Brand,
            partialRequest.Color,
            partialRequest.Model,
            partialRequest.FuelType,
            partialRequest.CapacityInLiters,
            partialRequest.Year,
            partialRequest.PhotoPath,
            partialRequest.GroupId
        );

        Result<UpdateVehicleResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteVehicleRequest request = new(id);

        Result<DeleteVehicleResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}
