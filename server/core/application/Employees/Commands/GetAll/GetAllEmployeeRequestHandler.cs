using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Employees;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public class GetAllEmployeeRequestHandler(
    IMapper mapper,
    IRepositoryEmployee repositoryEmployee,
    ILogger<GetAllEmployeeRequestHandler> logger
) : IRequestHandler<GetAllEmployeeRequest, Result<GetAllEmployeeResponse>>
{
    public async Task<Result<GetAllEmployeeResponse>> Handle(
        GetAllEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Employee> employees =
                request.Quantity.HasValue && request.Quantity.Value > 0 ?
                await repositoryEmployee.GetAllAsync(request.Quantity.Value) :
                await repositoryEmployee.GetAllAsync();

            GetAllEmployeeResponse response = mapper.Map<GetAllEmployeeResponse>(employees);

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
