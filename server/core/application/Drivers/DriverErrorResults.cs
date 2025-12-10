using FluentResults;

namespace LocadoraDeAutomoveis.Application.Drivers;

public abstract class DriverErrorResults
{
    public static Error DocumentAlreadyRegistredError(string document)
    {
        return new Error("Document already has been registred")
            .CausedBy($"A driver with the document {document} has already been registred")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error IndividualClientIdError()
    {
        return new Error("Individual Client Needed")
            .CausedBy($"The Business Client needs a Individual Client to be the Driver")
            .WithMetadata("ErrorType", "BadRequest");
    }
}