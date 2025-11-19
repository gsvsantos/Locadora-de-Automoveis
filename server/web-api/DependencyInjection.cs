using FluentValidation;
using Hangfire;
using LocadoraDeAutomoveis.Core.Domain.Shared;
using LocadoraDeAutomoveis.Infrastructure.ORM.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

namespace LocadoraDeAutomoveis.WebAPI;

public static class DependencyInjection
{
    public static void ConfigureDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        string? connectionString = configuration["SQL_CONNECTION_STRING"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("The SQL_CONNECTION_STRING variable was not provided.");
        }

        services.AddDbContext<IUnitOfWork, AppDbContext>(options =>
        {
            if (!environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging(false);
            }
            options.UseSqlServer(connectionString, opt =>
            {
                opt.EnableRetryOnFailure(3);
            });

        });
    }

    public static void ConfigureCorsPolicy(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration
    )
    {
        services.AddCors(options =>
        {
            if (environment.IsDevelopment())
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }
            else
            {
                string? origensPermitidasString = configuration["CORS_ALLOWED_ORIGINS"];

                if (string.IsNullOrWhiteSpace(origensPermitidasString))
                {
                    throw new Exception("The environment variable \"CORS_ALLOWED_ORIGINS\" was not provided.");
                }

                string[] origensPermitidas = origensPermitidasString
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(x => x.TrimEnd('/'))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .WithOrigins(origensPermitidas)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            }
        });
    }

    public static void ConfigureOpenApiAuthHeaders(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Locadora de Automóveis API", Version = "v1" });

            options.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "time-span",
                Example = new Microsoft.OpenApi.Any.OpenApiString("00:00:00")
            });

            options.MapType<Guid>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "guid",
                Example = new Microsoft.OpenApi.Any.OpenApiString("00000000-0000-0000-0000-000000000000")
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Inform the JWT TOKEN in the format {Bearer token}",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        //services.AddScoped<IExampleRepository, ExampleRepository>();
    }

    public static void ConfigureSerilog(this IServiceCollection services, ILoggingBuilder logging, IConfiguration configuration)
    {
        string? licenseKey = configuration["NEWRELIC_LICENSE_KEY"];

        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            throw new Exception("The NEWRELIC_LICENSE_KEY variable was not provided.");
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.NewRelicLogs(
                endpointUrl: "https://log-api.newrelic.com/log/v1",
                applicationName: "locadora-de-automoveis-api",
                licenseKey: licenseKey
            )
            .CreateLogger();

        logging.ClearProviders();

        services.AddLogging(builder => builder.AddSerilog(dispose: true));
    }

    public static void ConfigureServices(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        services.ConfigureHangFire(configuration);
        services.ConfigureRedisCache(configuration);
    }

    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    }

    private static void ConfigureHangFire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
            config.UseSqlServerStorage(configuration["HANGFIRE_SQL_CONNECTION_STRING"]));

        services.AddHangfireServer();
    }

    private static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        string? redisConnectionString = configuration["REDIS_CONNECTION_STRING"];

        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = redisConnectionString;
            option.InstanceName = "locadoradeautomoveis";
        });
    }
}
