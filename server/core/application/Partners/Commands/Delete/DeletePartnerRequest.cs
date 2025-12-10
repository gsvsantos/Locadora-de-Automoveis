using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.Delete;

public record DeletePartnerRequest(
    Guid Id
) : IRequest<Result<DeletePartnerResponse>>;
