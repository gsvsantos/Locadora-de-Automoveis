using FluentResults;

namespace LocadoraDeAutomoveis.Application.Vehicles;

public abstract class VehicleErrorResults
{
    public static Error DuplicateLicensePlateError(string licensePlate)
    {
        return new Error("Duplicate license plate")
            .CausedBy($"A vehicle with the license plate '{licensePlate}' has already been registered")
            .WithMetadata("ErrorType", "BadRequest");
    }

    public static Error VehicleUnavailable(Guid vehicleId)
    {
        return new Error("Vehicle unavailable")
            .CausedBy($"The vehicle with ID '{vehicleId}' is currently rented or unavailable.")
            .WithMetadata("ErrorType", "Conflict");
    }
}
