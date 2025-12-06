using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Create;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Delete;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/coupon")]
[Authorize("AdminOrEmployeePolicy")]
public class CouponController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCouponRequest request)
    {
        Result<CreateCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("most-used")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> GetMostUsed()
    {
        GetMostUsedCouponsRequest request = new();

        Result<GetMostUsedCouponsResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponRequestPartial partialRequest)
    {

        UpdateCouponRequest request = mapper.Map<UpdateCouponRequest>((partialRequest, id));

        Result<UpdateCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteCouponRequest request = new(id);

        Result<DeleteCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}