using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;

public record GetAllBillingPlanRequest(
    int? Quantity
) : IRequest<Result<GetAllBillingPlanResponse>>;
