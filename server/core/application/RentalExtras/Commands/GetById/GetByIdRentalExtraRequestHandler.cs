using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetById;

public class GetByIdRentalExtraRequestHandler(
    IMapper mapper,
    IRepositoryRentalExtra repositoryRentalExtra,
    ILogger<GetByIdRentalExtraRequestHandler> logger
) : IRequestHandler<GetByIdRentalExtraRequest, Result<GetByIdRentalExtraResponse>>
{
    public async Task<Result<GetByIdRentalExtraResponse>> Handle(
        GetByIdRentalExtraRequest request, CancellationToken cancellationToken)
    {
        try
        {
            RentalExtra? selectedRentalExtra = await repositoryRentalExtra.GetByIdAsync(request.Id);

            if (selectedRentalExtra is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdRentalExtraResponse response = mapper.Map<GetByIdRentalExtraResponse>(selectedRentalExtra);

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
