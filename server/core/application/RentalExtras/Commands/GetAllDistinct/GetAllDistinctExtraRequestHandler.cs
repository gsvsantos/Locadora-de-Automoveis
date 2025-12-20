using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAllDistinct;

public class GetAllDistinctExtraRequestHandler(
    IMapper mapper,
    IRepositoryRentalExtra repositoryRentalExtra,
    IRepositoryVehicle repositoryVehicle,
    ILogger<GetAllDistinctExtraRequestHandler> logger
) : IRequestHandler<GetAllDistinctExtraRequest, Result<GetAllDistinctExtraResponse>>
{
    public async Task<Result<GetAllDistinctExtraResponse>> Handle(
        GetAllDistinctExtraRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Vehicle? vehicle = await repositoryVehicle.GetByIdDistinctAsync(request.VehicleId);

            if (vehicle is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.VehicleId));
            }

            Guid tenantId = vehicle.GetTenantId();

            List<RentalExtra> extras = await repositoryRentalExtra.GetAllByTenantDistinctAsync(tenantId);

            GetAllDistinctExtraResponse response = mapper.Map<GetAllDistinctExtraResponse>(extras);

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
