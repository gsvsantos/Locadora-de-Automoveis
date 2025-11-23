using FluentResults;

namespace LocadoraDeAutomoveis.Application.Clients;

public abstract class ClientErrorResults
{
    public static Error DocumentAlreadyRegistredError(string document)
    {
        return new Error("Document already has been registred")
            .CausedBy($"The document {document} has already been registred")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
