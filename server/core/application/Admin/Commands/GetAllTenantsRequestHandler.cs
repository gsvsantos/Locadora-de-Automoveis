using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Admin.Commands;

public class GetAllTenantsRequestHandler(
    UserManager<User> userManager,
    IMapper mapper,
    ILogger<GetAllTenantsRequestHandler> logger
) : IRequestHandler<GetAllTenantsRequest, Result<GetAllTenantsResponse>>
{
    public async Task<Result<GetAllTenantsResponse>> Handle(
        GetAllTenantsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            IList<User> adminUsersInRole = await userManager.GetUsersInRoleAsync(nameof(ERoles.Admin));

            List<(User User, IList<string> Roles)> adminUsersWithRoles = [];

            foreach (User adminUser in adminUsersInRole)
            {
                IList<string> roles = await userManager.GetRolesAsync(adminUser);
                adminUsersWithRoles.Add((adminUser, roles));
            }

            List<TenantDto> tenantDtos = mapper.Map<List<TenantDto>>(adminUsersWithRoles);
            List<TenantDto> visibleTenantList = tenantDtos
                .Where(dto => !dto.Roles.Contains("PlatformAdmin"))
                .OrderBy(dto => dto.DisplayName)
                .ToList();

            GetAllTenantsResponse response = mapper.Map<GetAllTenantsResponse>(visibleTenantList);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during the request. \n{@Request}.", request);
            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
