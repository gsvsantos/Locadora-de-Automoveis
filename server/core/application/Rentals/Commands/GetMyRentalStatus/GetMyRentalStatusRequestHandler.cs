using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalStatus;

public class GetMyRentalStatusRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryClient repositoryClient,
    IRepositoryRental repositoryRental,
    IUserContext userContext,
    ILogger<GetMyRentalStatusRequestHandler> logger
) : IRequestHandler<GetMyRentalStatusRequest, Result<GetMyRentalStatusResponse>>
{
    public async Task<Result<GetMyRentalStatusResponse>> Handle(
        GetMyRentalStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid loginUserId = userContext.GetUserId();

            Client? client = await repositoryClient.GetGlobalByLoginUserIdAsync(loginUserId);

            if (client is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("Client profile not found."));
            }

            if (!client.HasFullProfile())
            {
                return Result.Ok(new GetMyRentalStatusResponse(
                    false,
                    ERentalStatusBlockReason.ProfileIncomplete,
                    null
                ));
            }

            Rental? rental = await repositoryRental.GetActiveRentalByLoginUserDistinctAsync(loginUserId);

            if (rental is not null)
            {
                return Result.Ok(mapper.Map<GetMyRentalStatusResponse>(rental));
            }

            return Result.Ok(new GetMyRentalStatusResponse(true, null, null));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred while getting rental status. {@Request}", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
