using FluentResults;
using LocadoraDeAutomoveis.Application.Configurations.Commands.Configure;
using LocadoraDeAutomoveis.Application.Configurations.Commands.Details;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/configuration")]
[Authorize("AdminOrEmployeePolicy")]
public class ConfigurationController(IMediator mediator) : ControllerBase
{
    [HttpPost("configure")]
    public async Task<IActionResult> Configure([FromBody] ConfigureRequest request)
    {
        Result<ConfigureResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("details")]
    public async Task<IActionResult> Details([FromQuery] DetailsRequest? request)
    {
        Result<DetailsResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }
        return Ok(result.Value);
    }
}
