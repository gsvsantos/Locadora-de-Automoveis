using AutoMapper;
using LocadoraDeAutomoveis.Application.Auth.Commands.ChangePassword;
using LocadoraDeAutomoveis.Application.Auth.Commands.CreatePassword;

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
    }
}
