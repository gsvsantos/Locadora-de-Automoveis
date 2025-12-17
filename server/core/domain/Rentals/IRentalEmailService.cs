using LocadoraDeAutomoveis.Domain.Clients;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public interface IRentalEmailService
{
    Task ScheduleRentalConfirmation(Rental rental, Client client);
}
