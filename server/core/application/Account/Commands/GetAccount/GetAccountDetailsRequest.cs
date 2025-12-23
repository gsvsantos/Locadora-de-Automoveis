using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetAccount;

public record GetAccountDetailsRequest() : IRequest<Result<GetAccountDetailsResponse>>;
