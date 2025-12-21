using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LocadoraDeAutomoveis.Application.Admin.Commands.Impersonate;

public class ImpersonateTenantRequestHandler(
    IUserContext userContext,
    UserManager<User> userManager,
    ITokenProvider tokenProvider
) : IRequestHandler<ImpersonateTenantRequest, Result<ImpersonateTenantResponse>>
{
    public async Task<Result<ImpersonateTenantResponse>> Handle(
        ImpersonateTenantRequest request, CancellationToken cancellationToken)
    {
        if (!userContext.IsInRole("PlatformAdmin"))
        {
            return Result.Fail("Not allowed.");
        }

        Guid? actorUserId = userContext.UserId;
        if (actorUserId is null)
        {
            return Result.Fail("Invalid actor.");
        }

        User? actorUser = await userManager.FindByIdAsync(actorUserId.Value.ToString());
        if (actorUser is null)
        {
            return Result.Fail("Actor not found.");
        }

        User? targetTenantAdminUser = await userManager.FindByIdAsync(request.TenantId.ToString());
        if (targetTenantAdminUser is null || targetTenantAdminUser.TenantId != request.TenantId)
        {
            return Result.Fail("Tenant not found.");
        }

        ImpersonationActorDto actor = new(
            actorUser.Id,
             actorUser.TenantId.GetValueOrDefault(),
            actorUser.UserName ?? string.Empty,
            actorUser.Email ?? string.Empty
        );

        AccessToken? accessToken = await tokenProvider.GenerateAccessToken(targetTenantAdminUser, actor) as AccessToken;

        if (accessToken is null)
        {
            return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token. Try again!")));
        }

        return Result.Ok(new ImpersonateTenantResponse(accessToken));
    }
}
