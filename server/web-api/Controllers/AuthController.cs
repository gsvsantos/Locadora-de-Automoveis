using FluentResults;
using LocadoraDeAutomoveis.Core.Application.Auth.Commands.Login;
using LocadoraDeAutomoveis.Core.Application.Auth.Commands.Register;
using LocadoraDeAutomoveis.Core.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Core.Domain.Auth;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator, SignInManager<User> signInManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        Result<TokenResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        Result<TokenResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}
