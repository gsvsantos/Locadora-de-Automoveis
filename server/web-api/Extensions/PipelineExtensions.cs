using Hangfire;
using LocadoraDeAutomoveis.WebAPI.Configuration;

namespace LocadoraDeAutomoveis.WebAPI.Extensions;

public static class PipelineExtensions
{
    public static WebApplication UseWebApiPipelineDefaults(this WebApplication app)
    {

        app.UseGlobalExceptionHandler();

        app.UseHangfireDashboard();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health/live");
        app.MapHealthChecks("/health/ready");

        return app;
    }
}
