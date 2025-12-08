using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;

public record GetAllPartnerRequestPartial(
    int? Quantity,
    bool? IsActive
);

public record GetAllPartnerRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllPartnerResponse>>;