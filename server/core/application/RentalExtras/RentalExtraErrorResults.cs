using FluentResults;

namespace LocadoraDeAutomoveis.Application.RentalExtras;
public abstract class RentalExtraErrorResults
{
    public static Error DuplicateNameError(string name)
    {
        return new Error("Duplicate name")
            .CausedBy($"An rental extra with the name '{name}' has already been registered")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
