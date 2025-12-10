using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.GlobalSearch.Commands;

public class GlobalSearchRequestHandler
    (IRepositoryEmployee repositoryEmployee,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryClient repositoryClient,
    IRepositoryDriver repositoryDriver,
    IRepositoryCoupon repositoryCoupon,
    IRepositoryRental repositoryRental,
    IRepositoryGroup repositoryGroup,
    ILogger<GlobalSearchRequestHandler> logger
) : IRequestHandler<GlobalSearchRequest, Result<GlobalSearchResponse>>
{
    public async Task<Result<GlobalSearchResponse>> Handle(
        GlobalSearchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Term))
            {
                return Result.Ok(new GlobalSearchResponse([]));
            }

            string term = request.Term.Trim().ToLower();

            List<Employee> employees = await repositoryEmployee.SearchAsync(term, cancellationToken);
            List<Vehicle> vehicles = await repositoryVehicle.SearchAsync(term, cancellationToken);
            List<Client> clients = await repositoryClient.SearchAsync(term, cancellationToken);
            List<Driver> drivers = await repositoryDriver.SearchAsync(term, cancellationToken);
            List<Coupon> coupons = await repositoryCoupon.SearchAsync(term, cancellationToken);
            List<Rental> rentals = await repositoryRental.SearchAsync(term, cancellationToken);
            List<Group> groups = await repositoryGroup.SearchAsync(term, cancellationToken);

            List<GlobalSearchItemDto> items = [];

            items.AddRange(employees.Select(e => new GlobalSearchItemDto(
                e.Id,
                e.FullName,
                e.User!.Email!,
                "Employee",
                $"/employees/edit/{e.Id}"
            )));

            items.AddRange(vehicles.Select(v => new GlobalSearchItemDto(
                v.Id,
                $"{v.Brand} {v.Model}",
                v.LicensePlate,
                "Vehicle",
                $"/vehicles/edit/{v.Id}"
            )));

            items.AddRange(clients.Select(c => new GlobalSearchItemDto(
                c.Id,
                c.FullName,
                c.Type == EClientType.Business ? $"Business - {c.Document}" : $"Individual - {c.Document}",
                "Client",
                $"/clients/edit/{c.Id}"
            )));

            items.AddRange(drivers.Select(d => new GlobalSearchItemDto(
                d.Id,
                d.FullName,
                $"Condutor (License: {d.LicenseNumber})",
                "Driver",
                $"/drivers/details/{d.Id}"
            )));

            items.AddRange(coupons.Select(c => new GlobalSearchItemDto(
                c.Id,
                $"{c.Name} ({c.DiscountValue:C})",
                $"Cupom - Parceiro: {c.Partner?.FullName ?? "N/A"}",
                "Coupon",
                $"/coupons/edit/{c.Id}"
            )));

            items.AddRange(rentals.Select(r => new GlobalSearchItemDto(
                r.Id,
                "Aluguel em andamento",
                $"{r.Client?.FullName} - {r.Vehicle?.Model} ({r.StartDate:dd/MM})",
                "Rental",
                $"/rentals/details/{r.Id}"
            )));

            items.AddRange(groups.Select(g => new GlobalSearchItemDto(
                g.Id,
                g.Name,
                "Grupo de Veículos",
                "Group",
                $"/groups/edit/{g.Id}"
            )));

            return Result.Ok(new GlobalSearchResponse(items));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred during the global search with term: {Term}",
                request.Term
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
