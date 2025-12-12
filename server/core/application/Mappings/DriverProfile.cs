using AutoMapper;
using LocadoraDeAutomoveis.Application.Drivers.Commands.Create;
using LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Drivers.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class DriverProfile : Profile
{
    public DriverProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllDriverRequestPartial, GetAllDriverRequest>()
            .ConvertUsing(src => new GetAllDriverRequest(
                src.Quantity,
                src.IsActive
            ));

        // DTOs
        CreateMap<Client, DriverClientDto>()
            .ConvertUsing(src => new DriverClientDto(
                src.Id,
                src.FullName,
                src.Type
            ));

        CreateMap<Driver, DriverDto>()
            .ConvertUsing((src, dest, ctx) => new DriverDto(
                src.Id,
                src.FullName,
                src.Email,
                src.PhoneNumber,
                src.Document,
                src.LicenseNumber,
                src.LicenseValidity,
                ctx.Mapper.Map<DriverClientDto>(src.Client),
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<CreateDriverRequest, Driver>()
            .ConvertUsing(src => new Driver(
                src.FullName,
                src.Email,
                src.PhoneNumber,
                src.Document,
                src.LicenseNumber,
                src.LicenseValidity
            ));

        CreateMap<(CreateDriverRequest r, Address a), Client>()
            .ConvertUsing(src => new Client(
                src.r.FullName,
                src.r.Email,
                src.r.PhoneNumber,
                src.r.Document,
                src.a
            ));

        // GetAll
        CreateMap<List<Driver>, GetAllDriverResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllDriverResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<DriverDto>(c)).ToImmutableList()
                    ?? ImmutableList<DriverDto>.Empty
            ));

        // GetById
        CreateMap<Driver, GetByIdDriverResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdDriverResponse(
               ctx.Mapper.Map<DriverDto>(src)
            ));
    }
}
