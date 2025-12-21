using Hangfire;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared.Email;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace LocadoraDeAutomoveis.Application.Rentals.Services;

public class RentalEmailService(
    IEmailSender emailSender,
    IEmailTemplateService templateService,
    IOptions<AppUrlsOptions> appUrlsOptions
) : IRentalEmailService
{
    private readonly AppUrlsOptions appUrls = appUrlsOptions.Value;

    public async Task ScheduleRentalConfirmation(Rental rental, Client client)
    {
        string language = client.PreferredLanguage ?? "pt-BR";
        CultureInfo culture = CultureInfo.GetCultureInfo(language);

        Dictionary<string, string> placeholders = new()
        {
            { "ClientName", client.FullName },
            { "CarModel", rental.Vehicle.Model },
            { "StartDate", rental.StartDate.ToString("D", culture) },
            { "ReturnDate", rental.ExpectedReturnDate.ToString("D", culture) },
            { "EstimatedPrice", rental.BaseRentalPrice.ToString("C", culture) },
            { "RentalLink", $"{this.appUrls.PortalApp}/rentals/details/{rental.Id}" }
        };

        string body = await templateService.GetTemplateAsync("rental-confirmation", placeholders, language);
        string subject = GetSubject("rental-confirmation", language, rental.Vehicle.Model);

        BackgroundJob.Enqueue(() => emailSender.SendAsync(client.Email, subject, body));

        DateTimeOffset reminderDate = rental.ExpectedReturnDate.AddDays(-1);
        if (reminderDate > DateTime.UtcNow)
        {
            BackgroundJob.Schedule<IRentalEmailService>(
                svc => svc.SendReturnReminder(
                    client.Email, client.FullName,
                    rental.Vehicle.Model, rental.ExpectedReturnDate,
                    rental.Id, language
                ),
                reminderDate
            );
        }
    }

    public async Task SendReturnReminder(string email, string name, string carModel, DateTimeOffset expectedReturnDate, Guid rentalId, string language)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(language);

        Dictionary<string, string> placeholders = new()
        {
            { "ClientName", name },
            { "CarModel", carModel },
            { "ReturnDate", expectedReturnDate.ToString("f", culture) },
            { "RentalLink", $"{this.appUrls.PortalApp}/rentals/details/{rentalId}" }
        };

        string body = await templateService.GetTemplateAsync("return-reminder", placeholders, language);
        string subject = GetSubject("return-reminder", language, carModel);

        await emailSender.SendAsync(email, subject, body);
    }

    private static string GetSubject(string templateKey, string language, string carModel)
    {
        return (templateKey, language) switch
        {
            ("rental-confirmation", "pt-BR") => $"Reserva confirmada: {carModel} - LDA",
            ("rental-confirmation", "es-ES") => $"Reserva confirmada: {carModel} - LDA",
            ("rental-confirmation", _) => $"Booking Confirmed: {carModel} - LDA",

            ("return-reminder", "pt-BR") => $"Lembrete: devolução amanhã - {carModel} - LDA",
            ("return-reminder", "es-ES") => $"Recordatorio: devolución mañana - {carModel} - LDA",
            ("return-reminder", _) => $"Reminder: {carModel} Return Tomorrow - LDA",

            _ => "LDA"
        };
    }
}