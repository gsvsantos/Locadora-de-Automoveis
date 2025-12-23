using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetAccountDetails;

public record GetAccountDetailsRequest() : IRequest<Result<GetAccountDetailsResponse>>;
