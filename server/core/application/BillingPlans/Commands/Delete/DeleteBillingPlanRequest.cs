using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Delete;

public record DeleteBillingPlanRequest(
    Guid Id
) : IRequest<Result<DeleteBillingPlanResponse>>;