using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Configurations.Commands.Details;

public class DetailsRequestHandler(
    IRepositoryConfiguration repositoryConfiguration,
    ITenantProvider tenantProvider,
    ILogger<DetailsRequestHandler> logger
) : IRequestHandler<DetailsRequest, Result<DetailsResponse>>
{
    public async Task<Result<DetailsResponse>> Handle(
        DetailsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid tenantId = tenantProvider.GetTenantId();
            Configuration? configuration = await repositoryConfiguration.GetByTenantId(tenantId);

            if (configuration is null)
            {
                return Result.Fail(ConfigurationErrorResults.NotFoundForTenant(tenantId));
            }

            DetailsResponse response = new(
                new ConfigurationDto(
                    configuration.Id,
                    configuration.GasolinePrice,
                    configuration.GasPrice,
                    configuration.DieselPrice,
                    configuration.AlcoholPrice
                )
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving Configuration for current Tenant");

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
