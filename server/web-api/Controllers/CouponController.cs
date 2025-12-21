using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Create;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Delete;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllDistinct;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetById;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;
using LocadoraDeAutomoveis.Application.Coupons.Commands.Update;
using LocadoraDeAutomoveis.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebApi.Controllers;

[ApiController]
[Route("api/coupon")]
public class CouponController(
    IMediator mediator,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("create")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Create([FromBody] CreateCouponRequest request)
    {
        Result<CreateCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> GetById(Guid id)
    {
        GetByIdCouponRequest request = new(id);

        Result<GetByIdCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("get-all")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllCouponRequestPartial partialRequest)
    {
        GetAllCouponRequest request = mapper.Map<GetAllCouponRequest>(partialRequest);

        Result<GetAllCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("most-used")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> GetMostUsed()
    {
        GetMostUsedCouponRequest request = new();

        Result<GetMostUsedCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpPut("update/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponRequestPartial partialRequest)
    {

        UpdateCouponRequest request = mapper.Map<UpdateCouponRequest>((partialRequest, id));

        Result<UpdateCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize("AdminOrEmployeePolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        DeleteCouponRequest request = new(id);

        Result<DeleteCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }

    [HttpGet("available/vehicle/{vehicleId:guid}")]
    [Authorize("EveryonePolicy")]
    public async Task<IActionResult> GetAllDistinct(Guid vehicleId)
    {
        GetAllDistinctCouponRequest request = new(vehicleId);

        Result<GetAllDistinctCouponResponse> result = await mediator.Send(request);

        return result.ToHttpResponse();
    }
}