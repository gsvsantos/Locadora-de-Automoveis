using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;

public record GetAllPartnerRequest(
    int? Quantity
) : IRequest<Result<GetAllPartnerResponse>>;