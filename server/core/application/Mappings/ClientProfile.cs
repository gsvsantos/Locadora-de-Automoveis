using AutoMapper;
using LocadoraDeAutomoveis.Application.Clients.Commands.Create;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetById;
using LocadoraDeAutomoveis.Application.Clients.Commands.Update;
using LocadoraDeAutomoveis.Domain.Clients;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllClientRequestPartial, GetAllClientRequest>()
            .ConvertUsing(src => new GetAllClientRequest(
                src.Quantity,
                src.IsActive
            ));

        // Update
        CreateMap<(UpdateClientRequestPartial p, Guid id), UpdateClientRequest>()
            .ConvertUsing(src => new UpdateClientRequest(
                src.id,
                src.p.FullName,
                src.p.Email,
                src.p.PhoneNumber,
                src.p.State,
                src.p.City,
                src.p.Neighborhood,
                src.p.Street,
                src.p.Number,
                src.p.Type,
                src.p.Document
            ));

        // DTOs
        CreateMap<Client, ClientDto>()
            .ConvertUsing(src => new ClientDto(
                src.Id,
                src.FullName,
                src.Email,
                src.PhoneNumber,
                src.Document ?? string.Empty,
                src.Address,
                src.Type,
                src.LicenseNumber ?? string.Empty,
                src.IsActive
            ));

        // HANDLERS 
        // Create
        CreateMap<CreateClientRequest, Address>()
            .ConvertUsing(src => new Address(
                src.State,
                src.City,
                src.Neighborhood,
                src.Street,
                src.Number
            ));
        CreateMap<(CreateClientRequest r, Address a), Client>()
            .ConvertUsing(src => new Client(
                src.r.FullName,
                src.r.Email,
                src.r.PhoneNumber,
                src.r.Document,
                src.a
            ));

        // GetALl
        CreateMap<List<Client>, GetAllClientResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllClientResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<ClientDto>(c)).ToImmutableList()
                    ?? ImmutableList<ClientDto>.Empty
            ));

        // GetById
        CreateMap<Client, GetByIdClientResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdClientResponse(
               ctx.Mapper.Map<ClientDto>(src)
            ));

        // Update
        CreateMap<UpdateClientRequest, Address>()
            .ConvertUsing(src => new Address(
                src.State,
                src.City,
                src.Neighborhood,
                src.Street,
                src.Number
            ));
        CreateMap<(UpdateClientRequest r, Address a), Client>()
            .ConvertUsing(src => new Client(
                src.r.FullName,
                src.r.Email,
                src.r.PhoneNumber,
                src.r.Document,
                src.a
            )
            { Id = src.r.Id });
    }
}
