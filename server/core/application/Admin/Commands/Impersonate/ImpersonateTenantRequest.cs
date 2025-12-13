using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Admin.Commands.Impersonate;

public record ImpersonateTenantRequest(
    Guid TenantId
) : IRequest<Result<ImpersonateTenantResponse>>;
