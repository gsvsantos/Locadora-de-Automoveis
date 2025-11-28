using FluentResults;
using LocadoraDeAutomoveis.Application.RateServices.Commands.Create;
using LocadoraDeAutomoveis.Application.RateServices.Commands.Delete;
using LocadoraDeAutomoveis.Application.RateServices.Commands.GetAll;
using LocadoraDeAutomoveis.Application.RateServices.Commands.GetById;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/rate-service")]
[Authorize("AdminOrEmployeePolicy")]
public class RateServiceController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRateServiceRequest request)
    {
        Result<CreateRateServiceResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRateServiceRequest? request)
    {
        request = request ?? new GetAllRateServiceRequest(null);

        Result<GetAllRateServiceResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }
        return Ok(result.Value);
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdRateServiceRequest request = new(id);

        Result<GetByIdRateServiceResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteRateServiceRequest request = new(id);

        Result<DeleteRateServiceResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}
