using FluentResults;

namespace LocadoraDeAutomoveis.Core.Application.Shared;

public abstract class ErrorResults
{
    public static Error ConflictError(string error)
    {
        return new Error("Conflict")
            .CausedBy(error)
            .WithMetadata("ErrorType", "Conflict");
    }

    public static Error BadRequestError(string error)
    {
        return new Error("Invalid request")
            .CausedBy(error)
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error BadRequestError(IEnumerable<string> errors)
    {
        return new Error("Invalid request")
            .CausedBy(errors)
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error DuplicateRecordError(string errorMessage)
    {
        return new Error("Duplicate record")
            .CausedBy(errorMessage)
            .WithMetadata("ErrorType", "DuplicateRecord");
    }

    public static Error NotFoundError(Guid id)
    {
        return new Error("Record not found")
            .CausedBy($"Could not retrieve record with ID: {id}")
            .WithMetadata("ErrorType", "NotFound");
    }

    public static Error NotFoundError(string record)
    {
        return new Error("Record not found")
            .CausedBy($"Could not retrieve record: {record}")
            .WithMetadata("ErrorType", "NotFound");
    }

    public static Error DeletionBlockedError(string errorMessage)
    {
        return new Error("Deletion blocked")
            .CausedBy(errorMessage)
            .WithMetadata("ErrorType", "DeletionBlocked");
    }

    public static Error InternalServerError(Exception ex)
    {
        return new Error("An internal server error occurred")
            .CausedBy(ex)
            .WithMetadata("ErrorType", "InternalServer");
    }
}
