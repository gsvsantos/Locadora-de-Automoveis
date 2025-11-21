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
public class EmployeeController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        Result<CreateEmployeeResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get-all")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> GetAll()
    {
        GetAllEmployeeRequest request = new();

        Result<GetAllEmployeeResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpGet("get/{id:guid}")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdEmployeeRequest request = new(id);

        Result<GetByIdEmployeeResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPut("update/{id:guid}")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequestPartial partialRequest)
    {

        UpdateEmployeeRequest request = new(
            id,
            partialRequest.FullName,
            partialRequest.AdmissionDate,
            partialRequest.Salary
        );

        Result<UpdateEmployeeResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpPut("self-update")]
    [Authorize("EmployeePolicy")]
    public async Task<IActionResult> SelfUpdate(
        [FromServices] IUserContext userContext,
        [FromBody] SelfUpdateEmployeeRequestPartial partialRequest
    )
    {
        SelfUpdateEmployeeRequest request = new(
            userContext.GetUserId(),
            partialRequest.FullName,
            partialRequest.AdmissionDate,
            partialRequest.Salary
        );

        Result<SelfUpdateEmployeeResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        DeleteEmployeeRequest request = new(id);

        Result<DeleteEmployeeResponse> result = await mediator.Send(request);

        if (result.IsFailed)
        {
            return this.MapFailure(result.ToResult());
        }

        return Ok(result.Value);
    }
}
