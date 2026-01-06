using LocadoraDeAutomoveis.Domain.Clients;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public interface IRentalEmailService
{
    Task ScheduleRentalConfirmation(Rental rental, Client client);

    Task SendReturnReminder(string email, string name, string carModel, DateTimeOffset returnDate, Guid rentalId, string language);
}
