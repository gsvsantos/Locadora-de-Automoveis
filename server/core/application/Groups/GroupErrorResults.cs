using FluentResults;

namespace LocadoraDeAutomoveis.Application.Groups;

public abstract class GroupErrorResults
{
    public static Error DuplicateNameError(string name)
    {
        return new Error("Duplicate name")
            .CausedBy($"A group with the name '{name}' has already been registered")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
