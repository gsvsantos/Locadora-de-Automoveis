using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Employees.Commands.Create;
using LocadoraDeAutomoveis.Application.Employees.Commands.Delete;
using LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Employees.Commands.GetById;
using LocadoraDeAutomoveis.Application.Employees.Commands.SelfUpdate;
using LocadoraDeAutomoveis.Application.Employees.Commands.Update;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/employee")]
public class EmployeeController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        Result<CreateEmployeeResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllEmployeeRequestPartial partialRequest)
    {
        GetAllEmployeeRequest request = mapper.Map<GetAllEmployeeRequest>(partialRequest);

        Result<GetAllEmployeeResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdEmployeeRequest request = new(id);

        Result<GetByIdEmployeeResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequestPartial partialRequest)
    {

        UpdateEmployeeRequest request = mapper.Map<UpdateEmployeeRequest>((partialRequest, id));

        Result<UpdateEmployeeResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("self-update")]
    [Authorize("EmployeePolicy")]
    public async Task<IActionResult> SelfUpdate(
        [FromServices] IUserContext userContext,
        [FromBody] SelfUpdateEmployeeRequestPartial partialRequest
    )
    {
        SelfUpdateEmployeeRequest request = mapper.Map<SelfUpdateEmployeeRequest>((partialRequest, userContext.GetUserId()));

        Result<SelfUpdateEmployeeResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        DeleteEmployeeRequest request = new(id);

        Result<DeleteEmployeeResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}
