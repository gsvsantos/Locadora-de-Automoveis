using FluentResults;
using LocadoraDeAutomoveis.Application.GlobalSearch.Commands;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/search")]
[Authorize]
public class SearchController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GlobalSearch([FromQuery] GlobalSearchRequest request)
    {
        Result<GlobalSearchResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}
