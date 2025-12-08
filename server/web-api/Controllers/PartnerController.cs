using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Partners.Commands.Create;
using LocadoraDeAutomoveis.Application.Partners.Commands.Delete;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using LocadoraDeAutomoveis.Application.Partners.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/partner")]
[Authorize("AdminOrEmployeePolicy")]
public class PartnerController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePartnerRequest request)
    {
        Result<CreatePartnerResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllPartnerRequestPartial partialRequest)
    {
        GetAllPartnerRequest request = mapper.Map<GetAllPartnerRequest>(partialRequest);

        Result<GetAllPartnerResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetCoupons(Guid id)
    {
        GetCouponsPartnerRequest request = new(id);

        Result<GetCouponsPartnerResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartnerRequestPartial partialRequest)
    {
        UpdatePartnerRequest request = mapper.Map<UpdatePartnerRequest>((partialRequest, id));

        Result<UpdatePartnerResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeletePartnerRequest request = new(id);

        Result<DeletePartnerResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}