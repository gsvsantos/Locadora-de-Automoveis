using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.Delete;

public class DeleteRateServiceRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryRateService repositoryRateService,
    ILogger<DeleteRateServiceRequestHandler> logger
) : IRequestHandler<DeleteRateServiceRequest, Result<DeleteRateServiceResponse>>
{
    public async Task<Result<DeleteRateServiceResponse>> Handle(
        DeleteRateServiceRequest request, CancellationToken cancellationToken)
    {
        RateService? selectedRateService = await repositoryRateService.GetByIdAsync(request.Id);

        if (selectedRateService is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        try
        {
            await repositoryRateService.DeleteAsync(request.Id);

            await unitOfWork.CommitAsync();

            return Result.Ok(new DeleteRateServiceResponse());
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
