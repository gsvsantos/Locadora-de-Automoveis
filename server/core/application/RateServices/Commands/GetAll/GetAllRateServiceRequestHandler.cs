using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RateServices;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.GetAll;

public class GetAllRateServiceRequestHandler(
    IMapper mapper,
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

            GetAllRateServiceResponse response = mapper.Map<GetAllRateServiceResponse>(rateServices);

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
