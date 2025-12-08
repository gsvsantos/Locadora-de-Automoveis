using FluentResults;

namespace LocadoraDeAutomoveis.Application.Rentals;

public abstract class RentalErrorResults
{
    public static Error RentalAlreadyBeenReturned(DateTimeOffset returnDate)
    {
        return new Error("Rental already returned")
            .CausedBy($"This rental has already been returned in {returnDate.Date.Day}")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
