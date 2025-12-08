using FluentResults;

namespace LocadoraDeAutomoveis.Application.Partners;

public abstract class PartnerErrorResults
{
    public static Error DuplicateNameError(string name)
    {
        return new Error("Duplicate name")
            .CausedBy($"An partner with the name '{name}' has already been registered")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
