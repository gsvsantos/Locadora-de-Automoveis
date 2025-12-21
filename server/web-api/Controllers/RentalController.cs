using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Application.Rentals.Commands.CreateSelfRental;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Delete;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalById;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentals;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalStatus;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Return;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/rental")]
public class RentalController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Create([FromBody] CreateRentalRequest request)
    {
        Result<CreateRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRentalRequestPartial partialRequest)
    {
        GetAllRentalRequest request = mapper.Map<GetAllRentalRequest>(partialRequest);

        Result<GetAllRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdRentalRequest request = new(id);

        Result<GetByIdRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRentalRequestPartial partialRequest)
    {
        UpdateRentalRequest request = mapper.Map<UpdateRentalRequest>((partialRequest, id));

        Result<UpdateRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteRentalRequest request = new(id);

        Result<DeleteRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPost("return/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Return(Guid id, ReturnRentalRequestPartial partialRequest)
    {
        ReturnRentalRequest request = mapper.Map<ReturnRentalRequest>((partialRequest, id));

        Result<ReturnRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPost("self")]
    [Authorize("EveryonePolicy")]
    public async Task<IActionResult> CreateSelfRental([FromBody] CreateSelfRentalRequest request)
    {
        Result<CreateRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("me")]
    [Authorize("EveryonePolicy")]
    public async Task<IActionResult> GetMyRentals([FromQuery] GetMyRentalsRequest request)
    {
        Result<GetMyRentalsResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("me/{id:guid}")]
    [Authorize("EveryonePolicy")]
    public async Task<IActionResult> GetMyRentalById(Guid id)
    {
        GetMyRentalByIdRequest request = new(id);

        Result<GetMyRentalByIdResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("me/status")]
    [Authorize("EveryonePolicy")]
    public async Task<IActionResult> GetMyRentalStatus()
    {
        GetMyRentalStatusRequest request = new();

        Result<GetMyRentalStatusResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}
