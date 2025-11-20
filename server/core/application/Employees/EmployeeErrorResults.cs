using FluentResults;

namespace LocadoraDeAutomoveis.Application.Employees;

public abstract class EmployeeErrorResults
{
    public static Error DuplicateNameError(string name)
    {
        return new Error("Duplicate name")
            .CausedBy($"An employee with the name '{name}' has already been registered")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
