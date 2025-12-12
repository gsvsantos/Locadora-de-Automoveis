using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Admin.Commands;

public record GetAllTenantsRequest(
) : IRequest<Result<GetAllTenantsResponse>>;
