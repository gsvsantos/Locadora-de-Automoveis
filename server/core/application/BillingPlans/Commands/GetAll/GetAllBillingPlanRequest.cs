using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;

public record GetAllBillingPlanRequestPartial(
    int? Quantity,
    bool? IsActive
);

public record GetAllBillingPlanRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllBillingPlanResponse>>;
