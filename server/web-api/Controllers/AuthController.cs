using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.Commands.Login;
using LocadoraDeAutomoveis.Application.Auth.Commands.LoginGoogle;
using LocadoraDeAutomoveis.Application.Auth.Commands.Logout;
using LocadoraDeAutomoveis.Application.Auth.Commands.Refresh;
using LocadoraDeAutomoveis.Application.Auth.Commands.Register;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(
    IMediator mediator,
    SignInManager<User> signInManager,
    IRefreshTokenCookieService cookieService
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        Result<(AccessToken AccessToken, RefreshToken RefreshToken)> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        return ResultWithNewCookie(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        Result<(AccessToken AccessToken, RefreshToken RefreshToken)> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        return ResultWithNewCookie(result.Value);
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] LoginWithGoogleRequest request)
    {
        Result<(AccessToken AccessToken, RefreshToken RefreshToken)> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        return ResultWithNewCookie(result.Value);
    }

    [HttpPost("refresh")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Refresh()
    {
        string? refreshTokenHash = cookieService.Get(this.Request);

        if (refreshTokenHash is null)
        {
            return Unauthorized("Refresh token not found.");
        }

        RefreshTokenRequest request = new(refreshTokenHash);

        Result<(AccessToken AccessToken, RefreshToken RefreshToken)> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        return ResultWithNewCookie(result.Value);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        string? refreshToken = cookieService.Get(this.Request);

        if (refreshToken is null)
        {
            return Unauthorized("Refresh token not found.");
        }

        LogoutUserRequest request = new(refreshToken);

        Result result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        await signInManager.SignOutAsync();

        return ResultAndClearCookie(result);
    }

    private OkObjectResult ResultWithNewCookie((AccessToken AccessToken, RefreshToken RefreshToken) value)
    {
        cookieService.Write(this.Response, value.RefreshToken);

        return Ok(value.AccessToken);
    }

    private NoContentResult ResultAndClearCookie(Result result)
    {
        cookieService.Remove(this.Response);

        return NoContent();
    }
}
