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

    public static Error ClientAlreadyHasActiveRentError()
    {
        return new Error("Client already has an active rental")
            .CausedBy("A client cannot start a new rental while another rental is still open.")
            .WithMetadata("ErrorType", "Conflict");
    }
}
