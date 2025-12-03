using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Drivers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;

public class GetAllDriverRequestHandler(
    IMapper mapper,
    IRepositoryDriver repositoryDriver,
    ILogger<GetAllDriverRequestHandler> logger
) : IRequestHandler<GetAllDriverRequest, Result<GetAllDriverResponse>>
{
    public async Task<Result<GetAllDriverResponse>> Handle(
        GetAllDriverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Driver> drivers =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryDriver.GetAllAsync(request.Quantity.Value)
                : await repositoryDriver.GetAllAsync();

            GetAllDriverResponse response = mapper.Map<GetAllDriverResponse>(drivers);

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
