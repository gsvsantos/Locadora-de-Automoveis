using Hangfire;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared.Email;

namespace LocadoraDeAutomoveis.Application.Rentals.Services;

public class RentalEmailService(
    IEmailSender emailSender,
    IEmailTemplateService templateService
) : IRentalEmailService
{
    public async Task ScheduleRentalConfirmation(Rental rental, Client client)
    {
        Dictionary<string, string> placeholders = new()
        {
            { "ClientName", client.FullName },
            { "CarModel", rental.Vehicle.Model },
            { "StartDate", rental.StartDate.ToString("MMM dd, yyyy") },
            { "ReturnDate", rental.ExpectedReturnDate.ToString("MMM dd, yyyy") },
            { "EstimatedPrice", rental.BaseRentalPrice.ToString("C") },
            { "RentalLink", "http://localhost:4200/rentals/details/" + rental.Id } // mudar antes do deploy na azure
        };

        string body = await templateService.GetTemplateAsync("rental-confirmation", placeholders);
        string subject = $"Booking Confirmed: {rental.Vehicle.Model} - LDA";

        BackgroundJob.Enqueue(() => emailSender.SendAsync(client.Email, subject, body));

        DateTimeOffset reminderDate = rental.ExpectedReturnDate.AddDays(-1);
        if (reminderDate > DateTime.UtcNow)
        {
            BackgroundJob.Schedule(
                () => SendReturnReminder(client.Email, client.FullName, rental.Vehicle.Model, rental.ExpectedReturnDate, rental.Id),
                reminderDate
            );
        }
    }

    public async Task SendReturnReminder(string email, string name, string carModel, DateTimeOffset returnDate, Guid rentalId)
    {
        Dictionary<string, string> placeholders = new()
        {
            { "ClientName", name },
            { "CarModel", carModel },
            { "ReturnDate", returnDate.ToString("MMM dd, yyyy 'at' HH:mm") },
            { "RentalLink", $"http://localhost:4200/rentals/details/{rentalId}" } // mudar antes do deploy na azure
        };

        string body = await templateService.GetTemplateAsync("return-reminder", placeholders);

        string subject = $"Reminder: {carModel} Return Tomorrow";

        await emailSender.SendAsync(email, subject, body);
    }
}