using FluentResults;
using LocadoraDeAutomoveis.Application.Admin.Commands.GetAll;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize("PlatformAdminPolicy")]
public sealed class AdminController(IMediator mediator) : ControllerBase
{
    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenants([FromQuery] GetAllTenantsRequest request)
    {
        Result<GetAllTenantsResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}