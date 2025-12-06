using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Groups.Commands.Create;
using LocadoraDeAutomoveis.Application.Groups.Commands.Delete;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetById;
using LocadoraDeAutomoveis.Application.Groups.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/group")]
[Authorize("AdminOrEmployeePolicy")]
public class GroupController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        Result<CreateGroupResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] int? quantity)
    {
        GetAllGroupRequest request = new(quantity);

        Result<GetAllGroupResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdGroupRequest request = new(id);

        Result<GetByIdGroupResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupRequestPartial partialRequest)
    {

        UpdateGroupRequest request = mapper.Map<UpdateGroupRequest>((partialRequest, id));

        Result<UpdateGroupResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteGroupRequest request = new(id);

        Result<DeleteGroupResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}
