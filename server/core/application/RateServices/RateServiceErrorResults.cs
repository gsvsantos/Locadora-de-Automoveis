using FluentResults;

namespace LocadoraDeAutomoveis.Application.RateServices;
public abstract class RateServiceErrorResults
{
    public static Error DuplicateNameError(string name)
    {
        return new Error("Duplicate name")
            .CausedBy($"An rate service with the name '{name}' has already been registered")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
