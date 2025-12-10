using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.Create;

public record CreatePartnerRequest(
    string FullName
) : IRequest<Result<CreatePartnerResponse>>;
