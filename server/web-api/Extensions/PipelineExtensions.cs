using Hangfire;
using LocadoraDeAutomoveis.WebAPI.Configuration;

namespace LocadoraDeAutomoveis.WebAPI.Extensions;

public static class PipelineExtensions
{
    public static async Task<WebApplication> UseWebApiPipelineDefaults(this WebApplication app)
    {
        app.AutoMigrateDatabase();
        app.StartRefreshTokenCleanupJob();

        await app.IdentitySeederAsync();

        app.UseGlobalExceptionHandler();

        app.UseHangfireDashboard(); ;

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
