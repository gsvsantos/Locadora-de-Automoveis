using FluentResults;
using LocadoraDeAutomoveis.Application.Account.Commands.GetAccount;
using LocadoraDeAutomoveis.Application.Account.Commands.UpdateAccount;
using LocadoraDeAutomoveis.Application.Account.Commands.UpdateLanguage;
using LocadoraDeAutomoveis.Infrastructure.Localization;
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
    [HttpGet("details")]
    public async Task<IActionResult> GetDetails()
    {
        GetAccountDetailsRequest request = new();

        Result<GetAccountDetailsResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateAccountRequest request)
    {
        Result<UpdateAccountResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("language")]
    public async Task<IActionResult> UpdateLanguage([FromBody] UpdateLanguageRequest request)
    {
        if (!LanguageCodes.IsSupported(request.Language))
        {
            return BadRequest("Unsupported language.");
        }

        Result<UpdateLanguageResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}