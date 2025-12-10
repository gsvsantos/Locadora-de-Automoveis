using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Drivers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetById;

public class GetByIdDriverRequestHandler(
    IMapper mapper,
    IRepositoryDriver repositoryDriver,
    ILogger<GetByIdDriverRequestHandler> logger
) : IRequestHandler<GetByIdDriverRequest, Result<GetByIdDriverResponse>>
{
    public async Task<Result<GetByIdDriverResponse>> Handle(
        GetByIdDriverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Driver? selectedDriver = await repositoryDriver.GetByIdAsync(request.Id);

            if (selectedDriver is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdDriverResponse response = mapper.Map<GetByIdDriverResponse>(selectedDriver);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving Driver by ID {DriverId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the selectedDriver."));
        }
    }
}
