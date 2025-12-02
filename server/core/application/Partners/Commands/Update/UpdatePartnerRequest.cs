using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.Update;

public record UpdatePartnerRequestPartial(
    string FullName
);

public record UpdatePartnerRequest(
    Guid Id,
    string FullName
) : IRequest<Result<UpdatePartnerResponse>>;
