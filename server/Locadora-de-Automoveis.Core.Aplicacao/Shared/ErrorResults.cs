using FluentResults;

namespace Locadora_de_Automoveis.Core.Aplicacao.Shared;

public abstract class ResultadosErro
{
    public static Error ConflictError(string error)
    {
        return new Error("Conflict")
            .CausedBy(error)
            .WithMetadata("ErrorType", "Conflict");
    }

    public static Error InvalidRequestError(string error)
    {
        return new Error("Invalid request")
            .CausedBy(error)
            .WithMetadata("ErrorType", "InvalidRequest");
    }

    public static Error InvalidRequestError(IEnumerable<string> errors)
    {
        return new Error("Invalid request")
            .CausedBy(errors)
            .WithMetadata("ErrorType", "InvalidRequest");
    }

    public static Error DuplicateRecordError(string errorMessage)
    {
        return new Error("Duplicate record")
            .CausedBy(errorMessage)
            .WithMetadata("ErrorType", "DuplicateRecord");
    }

    public static Error RecordNotFoundError(Guid id)
    {
        return new Error("Record not found")
            .CausedBy($"Could not retrieve record with ID: {id}")
            .WithMetadata("ErrorType", "RecordNotFound");
    }

    public static Error RecordNotFoundError(string record)
    {
        return new Error("Record not found")
            .CausedBy($"Could not retrieve record: {record}")
            .WithMetadata("ErrorType", "RecordNotFound");
    }

    public static Error DeletionBlockedError(string errorMessage)
    {
        return new Error("Deletion blocked")
            .CausedBy(errorMessage)
            .WithMetadata("ErrorType", "DeletionBlocked");
    }

    public static Error InternalExceptionError(Exception ex)
    {
        return new Error("An internal server error occurred")
            .CausedBy(ex)
            .WithMetadata("ErrorType", "InternalException");
    }
}
