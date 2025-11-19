using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace LocadoraDeAutomoveis.WebAPI.Configuration;

public static class GlobalExceptionHandlerConfig
{
    private static readonly string[] handler = ["Internal server error"];

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(builder =>
        {
            builder.Run(async httpContext =>
            {
                IExceptionHandlerFeature? gerenciadorExcecoes = httpContext.Features.Get<IExceptionHandlerFeature>();

                if (gerenciadorExcecoes is null)
                {
                    return;
                }

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var objeto = new
                {
                    Sucesso = false,
                    Erros = handler
                };

                string respostaJson = JsonSerializer.Serialize(objeto);

                await httpContext.Response.WriteAsync(respostaJson);
            });
        });
    }
}