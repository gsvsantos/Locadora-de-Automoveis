using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.Commands.ChangePassword;
using LocadoraDeAutomoveis.Application.Auth.Commands.CreatePassword;
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
    IMapper mapper,
    SignInManager<User> signInManager,
    IRefreshTokenCookieService cookieService
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        Result<(AccessToken AccessToken, IssuedRefreshTokenDto RefreshToken)> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        return ResultWithNewCookie(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        Result<(AccessToken AccessToken, IssuedRefreshTokenDto RefreshToken)> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        return ResultWithNewCookie(result.Value);
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] LoginWithGoogleRequest request)
    {
        Result<(AccessToken AccessToken, IssuedRefreshTokenDto RefreshToken)> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        return ResultWithNewCookie(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        string? refreshTokenPlain = cookieService.Get(this.Request);

        if (refreshTokenPlain is null)
        {
            return Unauthorized("Refresh token not found.");
        }

        RefreshTokenRequest request = new(refreshTokenPlain);

        Result<(AccessToken AccessToken, IssuedRefreshTokenDto NewRefreshToken)> result = await mediator.Send(request);

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

        return ResultAndClearCookie();
    }

    [HttpPost("create-password")]
    public async Task<IActionResult> CreatePassword([FromBody] CreatePasswordRequestPartial partialRequest)
    {
        string? refreshToken = cookieService.Get(this.Request);

        if (refreshToken is null)
        {
            return Unauthorized("Refresh token not found.");
        }

        CreatePasswordRequest request = mapper.Map<CreatePasswordRequest>((refreshToken, partialRequest));

        Result result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        await signInManager.SignOutAsync();

        return ResultAndClearCookie();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestPartial partialRequest)
    {
        string? refreshToken = cookieService.Get(this.Request);

        if (refreshToken is null)
        {
            return Unauthorized("Refresh token not found.");
        }

        ChangePasswordRequest request = mapper.Map<ChangePasswordRequest>((refreshToken, partialRequest));

        Result result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return result.ToHttpResponse();
        }

        await signInManager.SignOutAsync();

        return ResultAndClearCookie();
    }

    private OkObjectResult ResultWithNewCookie((AccessToken AccessToken, IssuedRefreshTokenDto RefreshToken) result)
    {
        cookieService.Write(this.Response, result.RefreshToken);

        return Ok(result.AccessToken);
    }

    private NoContentResult ResultAndClearCookie()
    {
        cookieService.Remove(this.Response);

        return NoContent();
    }
}
