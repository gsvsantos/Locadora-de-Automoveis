using FluentResults;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Delete;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Return;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/rental")]
[Authorize("AdminOrEmployeePolicy")]
public class RentalController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRentalRequest request)
    {
        Result<CreateRentalResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRentalRequest? request)
    {
        request = request ?? new GetAllRentalRequest(null);

        Result<GetAllRentalResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }
        return Ok(result.Value);
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdRentalRequest request = new(id);

        Result<GetByIdRentalResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRentalRequestPartial partialRequest)
    {

        UpdateRentalRequest request = new(
            id,
            partialRequest.StartDate,
            partialRequest.ExpectedReturnDate,
            partialRequest.StartKm,
            partialRequest.EmployeeId,
            partialRequest.ClientId,
            partialRequest.DriverId,
            partialRequest.VehicleId,
            partialRequest.CouponId,
            partialRequest.SelectedPlanType,
            partialRequest.EstimatedKilometers,
            partialRequest.RentalRateServicesIds
        );

        Result<UpdateRentalResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteRentalRequest request = new(id);

        Result<DeleteRentalResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPost("return/{id:guid}")]
    public async Task<IActionResult> Return(Guid id, ReturnRentalRequestPartial requestPartial)
    {
        ReturnRentalRequest request = new(
            id,
            requestPartial.EndKm,
            requestPartial.FuelLevel
        );

        Result<ReturnRentalResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}
