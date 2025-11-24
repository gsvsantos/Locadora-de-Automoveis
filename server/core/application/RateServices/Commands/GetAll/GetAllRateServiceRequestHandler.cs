using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RateServices;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.GetAll;

public class GetAllRateServiceRequestHandler(
    IRepositoryRateService repositoryRateService,
    ILogger<GetAllRateServiceRequestHandler> logger
) : IRequestHandler<GetAllRateServiceRequest, Result<GetAllRateServiceResponse>>
{
    public async Task<Result<GetAllRateServiceResponse>> Handle(
        GetAllRateServiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<RateService> rateServices =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryRateService.GetAllAsync(request.Quantity.Value)
                : await repositoryRateService.GetAllAsync();

            GetAllRateServiceResponse response = new(
                rateServices.Count,
                rateServices.Select(rateService => new RateServiceDto(
                    rateService.Id,
                    rateService.Name,
                    rateService.Price,
                    rateService.IsFixed
                )).ToImmutableList()
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
