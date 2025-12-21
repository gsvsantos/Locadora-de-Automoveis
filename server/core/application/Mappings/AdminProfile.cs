using AutoMapper;
using LocadoraDeAutomoveis.Application.Admin.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Auth;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        // DTOs
        CreateMap<(User User, IList<string> Roles), TenantDto>()
            .ConvertUsing(item => new TenantDto(
                    item.User.GetTenantId(),
                    item.User.Id,
                    item.User.FullName,
                    item.User.Email ?? string.Empty,
                    item.Roles
                ));

        // HANDLERS
        // GetTenants
        CreateMap<List<(User User, IList<string> Roles)>, List<TenantDto>>()
            .ConvertUsing((src, dest, ctx) =>
                [.. src.Select(c => ctx.Mapper.Map<TenantDto>(c)).ToList()
                ?? []]
            );

        CreateMap<List<TenantDto>, GetAllTenantsResponse>()
            .ConvertUsing(src =>
                new GetAllTenantsResponse(
                    src.Count,
                    src.ToImmutableList()
                )
            );
    }
}
