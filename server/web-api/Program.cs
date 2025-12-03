using LocadoraDeAutomoveis.WebAPI.Extensions;

namespace LocadoraDeAutomoveis.WebAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Database Provider [env SQL_CONNECTION_STRING]
        builder.Services.ConfigureDbContext(builder.Configuration, builder.Environment);

        // CORS [env CORS_ALLOWED_ORIGINS]
        builder.Services.ConfigureCorsPolicy(builder.Environment, builder.Configuration);

        // HealthChecks
        builder.Services.AddHealthChecks();
        builder.Services.AddProblemDetails();

        // API Documentation 
        builder.Services.ConfigureOpenApiAuthHeaders();

        // Add Scoped Dependencies
        builder.Services.ConfigureRepositories();
        builder.Services.ConfigureServices();

        // Logging [env NEWRELIC_LICENSE_KEY]
        builder.Services.ConfigureSerilog(builder.Logging, builder.Configuration);

        // Services
        builder.Services.ConfigureServices(builder.Configuration);

        // Auth [env JWT_GENERATION_KEY, JWT_AUDIENCE_DOMAIN]
        builder.Services.ConfigureIdentityProviders();
        builder.Services.ConfigureJwtAuthentication(builder.Configuration);

        // Controllers
        builder.Services.ConfigureControllers();

        WebApplication app = builder.Build();

        await app.UseWebApiPipelineDefaults();

        app.Run();
    }
}
