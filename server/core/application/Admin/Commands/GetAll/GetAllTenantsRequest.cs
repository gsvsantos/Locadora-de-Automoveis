using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Admin.Commands.GetAll;

public record GetAllTenantsRequest(
) : IRequest<Result<GetAllTenantsResponse>>;
