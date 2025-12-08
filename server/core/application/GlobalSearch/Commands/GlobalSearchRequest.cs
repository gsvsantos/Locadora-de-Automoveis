using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.GlobalSearch.Commands;

public record GlobalSearchRequest(
    string Term
) : IRequest<Result<GlobalSearchResponse>>;
