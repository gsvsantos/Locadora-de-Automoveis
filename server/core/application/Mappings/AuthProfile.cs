using AutoMapper;
using LocadoraDeAutomoveis.Application.Auth.Commands.ChangePassword;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        // CONTROLER
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
