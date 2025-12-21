using AutoMapper;
using Google.Apis.Auth;
using LocadoraDeAutomoveis.Application.Auth.Commands.ChangePassword;
using LocadoraDeAutomoveis.Application.Auth.Commands.CreatePassword;
using LocadoraDeAutomoveis.Application.Auth.Commands.RegisterClient;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        // CONTROLER
        // CreatePassword
        CreateMap<(string rt, CreatePasswordRequestPartial rp), CreatePasswordRequest>()
            .ConvertUsing(src => new CreatePasswordRequest(
            src.rt,
            src.rp.NewPassword,
            src.rp.ConfirmNewPassword
            ));

        // ChangePassword
        CreateMap<(string rt, ChangePasswordRequestPartial rp), ChangePasswordRequest>()
            .ConvertUsing(src => new ChangePasswordRequest(
            src.rt,
            src.rp.CurrentPassword,
            src.rp.NewPassword,
            src.rp.ConfirmNewPassword
            ));

        // HANDLERS 
        // RegisterClient
        CreateMap<RegisterClientRequest, User>()
            .ConvertUsing(src => new User()
            {
                UserName = src.UserName,
                FullName = src.FullName,
                Email = src.Email,
                PhoneNumber = src.PhoneNumber,
            });

        CreateMap<RegisterClientRequest, Client>()
            .ConvertUsing(src => new Client(
                src.FullName,
                src.Email,
                src.PhoneNumber
            ));

        // RegisterClientGoogle
        CreateMap<GoogleJsonWebSignature.Payload, User>()
            .ConvertUsing(src => new User()
            {
                UserName = src.Email,
                FullName = src.Name,
                Email = src.Email,
            });

        CreateMap<GoogleJsonWebSignature.Payload, Client>()
            .ConvertUsing(src => new Client(
                src.Name,
                src.Email,
                ""
            ));
    }
}
