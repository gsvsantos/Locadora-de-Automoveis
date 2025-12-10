using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;

public record GetCouponsPartnerRequest(
    Guid Id
) : IRequest<Result<GetCouponsPartnerResponse>>;
