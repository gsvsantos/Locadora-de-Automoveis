using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Account.Commands.UpdateLanguage;

public record UpdateLanguageRequest(
    string Language
) : IRequest<Result<UpdateLanguageResponse>>;
