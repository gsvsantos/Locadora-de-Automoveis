using AutoMapper;
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
public class RentalController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRentalRequest request)
    {
        Result<CreateRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] int? quantity)
    {
        GetAllRentalRequest request = new(quantity);

        Result<GetAllRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdRentalRequest request = new(id);

        Result<GetByIdRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRentalRequestPartial partialRequest)
    {

        UpdateRentalRequest request = mapper.Map<UpdateRentalRequest>((partialRequest, id));

        Result<UpdateRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteRentalRequest request = new(id);

        Result<DeleteRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPost("return/{id:guid}")]
    public async Task<IActionResult> Return(Guid id, ReturnRentalRequestPartial partialRequest)
    {
        ReturnRentalRequest request = mapper.Map<ReturnRentalRequest>((partialRequest, id));

        Result<ReturnRentalResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}
