using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetById;

public record GetByIdBillingPlanRequest(
    Guid Id
) : IRequest<Result<GetByIdBillingPlanResponse>>;