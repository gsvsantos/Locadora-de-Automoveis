using FluentResults;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetClientProfile;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/account")]
[Authorize("EveryonePolicy")]
public class AccountController(IMediator mediator) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile([FromQuery] GetClientProfileRequest request)
    {
        Result<GetClientProfileResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}