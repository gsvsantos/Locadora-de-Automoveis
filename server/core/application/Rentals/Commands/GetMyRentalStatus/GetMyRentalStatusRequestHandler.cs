using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Rentals.Commands.CreateSelfRental;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalStatus;

public class GetMyRentalStatusRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryRental repositoryRental,
    IUserContext userContext,
    ILogger<CreateSelfRentalRequestHandler> logger
) : IRequestHandler<GetMyRentalStatusRequest, Result<GetMyRentalStatusResponse>>
{
    public async Task<Result<GetMyRentalStatusResponse>> Handle(
        GetMyRentalStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid loginUserId = userContext.GetUserId();

            Rental? rental = await repositoryRental.GetActiveRentalByLoginUserDistinctAsync(loginUserId);

            if (rental is null)
            {
                return Result.Ok(new GetMyRentalStatusResponse(
                    true,
                    null,
                    null
                ));
            }

            GetMyRentalStatusResponse response = mapper.Map<GetMyRentalStatusResponse>(rental);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
