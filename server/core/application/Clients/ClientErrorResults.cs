using FluentResults;

namespace LocadoraDeAutomoveis.Application.Clients;

public abstract class ClientErrorResults
{
    public static Error DocumentAlreadyRegistredError(string document)
    {
        return new Error("Document already has been registred")
            .CausedBy($"A client with the document {document} has already been registred")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error ClientIsNotBusinessError(string fullName)
    {
        return new Error("Client type error")
            .CausedBy($"The client selected {fullName} is not a Business Client")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
