using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;

public class GetAllRentalExtraRequestHandler(
    IMapper mapper,
    IRepositoryRentalExtra repositoryRentalExtra,
    ILogger<GetAllRentalExtraRequestHandler> logger
) : IRequestHandler<GetAllRentalExtraRequest, Result<GetAllRentalExtraResponse>>
{
    public async Task<Result<GetAllRentalExtraResponse>> Handle(
        GetAllRentalExtraRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<RentalExtra> RentalExtras =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryRentalExtra.GetAllAsync(request.Quantity.Value)
                : await repositoryRentalExtra.GetAllAsync();

            GetAllRentalExtraResponse response = mapper.Map<GetAllRentalExtraResponse>(RentalExtras);

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
