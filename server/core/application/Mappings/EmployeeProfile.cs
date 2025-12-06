using AutoMapper;
using LocadoraDeAutomoveis.Application.Employees.Commands.Create;
using LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Employees.Commands.GetById;
using LocadoraDeAutomoveis.Application.Employees.Commands.SelfUpdate;
using LocadoraDeAutomoveis.Application.Employees.Commands.Update;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<(SelfUpdateEmployeeRequestPartial p, Guid id), SelfUpdateEmployeeRequest>()
            .ConvertUsing(src => new SelfUpdateEmployeeRequest(
                src.id,
                src.p.FullName
            ));

        // SelfUpdate
        CreateMap<GetAllEmployeeRequestPartial, GetAllEmployeeRequest>()
            .ConvertUsing(src => new GetAllEmployeeRequest(
                src.Quantity,
                src.IsActive
            ));

        // Update
        CreateMap<(UpdateEmployeeRequestPartial p, Guid id), UpdateEmployeeRequest>()
            .ConvertUsing(src => new UpdateEmployeeRequest(
                src.id,
                src.p.FullName,
                src.p.AdmissionDate,
                src.p.Salary
            ));

        // DTOs
        CreateMap<Employee, EmployeeDto>()
            .ConvertUsing(src => new EmployeeDto(
                src.Id,
                src.FullName,
                src.AdmissionDate,
                src.Salary,
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<CreateEmployeeRequest, User>()
            .ConvertUsing(src => new User()
            {
                UserName = src.UserName,
                FullName = src.FullName,
                Email = src.Email,
                PhoneNumber = src.PhoneNumber,
            });
        CreateMap<CreateEmployeeRequest, Employee>()
            .ConvertUsing(src => new Employee(
                src.FullName,
                src.AdmissionDate,
                src.Salary
            ));

        // GetALl
        CreateMap<List<Employee>, GetAllEmployeeResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllEmployeeResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<EmployeeDto>(c)).ToImmutableList()
                    ?? ImmutableList<EmployeeDto>.Empty
            ));

        // GetById
        CreateMap<Employee, GetByIdEmployeeResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdEmployeeResponse(
               ctx.Mapper.Map<EmployeeDto>(src)
            ));

        // SelfUpdate
        CreateMap<(SelfUpdateEmployeeRequest r, Employee e), Employee>()
            .ConvertUsing(src => new Employee(
                src.r.FullName,
                src.e.AdmissionDate,
                src.e.Salary
            )
            { Id = src.e.Id });

        // Update
        CreateMap<UpdateEmployeeRequest, Employee>()
            .ConvertUsing(src => new Employee(
                src.FullName,
                src.AdmissionDate,
                src.Salary
            )
            { Id = src.Id });
    }
}
