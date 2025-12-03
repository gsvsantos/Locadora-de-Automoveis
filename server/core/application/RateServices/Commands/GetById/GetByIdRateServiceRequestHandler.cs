using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RateServices;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.GetById;

public class GetByIdRateServiceRequestHandler(
    IMapper mapper,
    IRepositoryRateService repositoryRateService,
    ILogger<GetByIdRateServiceRequestHandler> logger
) : IRequestHandler<GetByIdRateServiceRequest, Result<GetByIdRateServiceResponse>>
{
    public async Task<Result<GetByIdRateServiceResponse>> Handle(
        GetByIdRateServiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            RateService? selectedRateService = await repositoryRateService.GetByIdAsync(request.Id);

            if (selectedRateService is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdRateServiceResponse response = mapper.Map<GetByIdRateServiceResponse>(selectedRateService);

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
