using AutoMapper;
using LocadoraDeAutomoveis.Application.Groups.Commands.Create;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetAllDistinct;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetById;
using LocadoraDeAutomoveis.Application.Groups.Commands.Update;
using LocadoraDeAutomoveis.Domain.Groups;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllGroupRequestPartial, GetAllGroupRequest>()
            .ConvertUsing(src => new GetAllGroupRequest(
                src.Quantity,
                src.IsActive
            ));

        // Update
        CreateMap<(UpdateGroupRequestPartial p, Guid id), UpdateGroupRequest>()
            .ConvertUsing(src => new UpdateGroupRequest(
                src.id,
                src.p.Name
            ));

        // DTOs
        CreateMap<Group, GroupDto>()
            .ConvertUsing(src => new GroupDto(
                src.Id,
                src.Name,
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<CreateGroupRequest, Group>()
            .ConvertUsing(src => new Group(
                src.Name
            ));

        // GetAll
        CreateMap<List<Group>, GetAllGroupResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllGroupResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<GroupDto>(c)).ToImmutableList()
                    ?? ImmutableList<GroupDto>.Empty
            ));

        // GetById
        CreateMap<Group, GetByIdGroupResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdGroupResponse(
               ctx.Mapper.Map<GroupDto>(src)
            ));

        // Update
        CreateMap<UpdateGroupRequest, Group>()
            .ConvertUsing(src => new Group(
                src.Name
            )
            { Id = src.Id });

        // GetAllDistinct
        CreateMap<List<Group>, GetAllDistinctGroupResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllDistinctGroupResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<GroupDto>(c)).ToImmutableList()
                    ?? ImmutableList<GroupDto>.Empty
            ));
    }
}
