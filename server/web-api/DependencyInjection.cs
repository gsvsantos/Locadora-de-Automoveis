using FluentValidation;
using Hangfire;
using LocadoraDeAutomoveis.Application.Employees.Commands.Create;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Infrastructure.Clients;
using LocadoraDeAutomoveis.Infrastructure.Configurations;
using LocadoraDeAutomoveis.Infrastructure.Drivers;
using LocadoraDeAutomoveis.Infrastructure.Employees;
using LocadoraDeAutomoveis.Infrastructure.Groups;
using LocadoraDeAutomoveis.Infrastructure.PricingPlans;
using LocadoraDeAutomoveis.Infrastructure.RateServices;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using LocadoraDeAutomoveis.Infrastructure.Vehicles;
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
            throw new Exception("The environment variable \"SQL_CONNECTION_STRING\" was not provided.");
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
                string? corsAllowedString = configuration["CORS_ALLOWED_ORIGINS"];

                if (string.IsNullOrWhiteSpace(corsAllowedString))
                {
                    throw new Exception("The environment variable \"CORS_ALLOWED_ORIGINS\" was not provided.");
                }

                string[] corsAllowed = corsAllowedString
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(x => x.TrimEnd('/'))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .WithOrigins(corsAllowed)
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
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Locadora de Veículos API", Version = "v1" });

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
        services.AddScoped<IRepositoryEmployee, EmployeeRepository>();
        services.AddScoped<IRepositoryGroup, GroupRepository>();
        services.AddScoped<IRepositoryVehicle, VehicleRepository>();
        services.AddScoped<IRepositoryPricingPlan, PricingPlanRepository>();
        services.AddScoped<IRepositoryClient, ClientRepository>();
        services.AddScoped<IRepositoryDriver, DriverRepository>();
        services.AddScoped<IRepositoryRateService, RateServiceRepository>();
        services.AddScoped<IRepositoryConfiguration, ConfigurationRepository>();
    }

    public static void ConfigureSerilog(this IServiceCollection services, ILoggingBuilder logging, IConfiguration configuration)
    {
        string? licenseKey = configuration["NEWRELIC_LICENSE_KEY"];

        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            throw new Exception("The environment variable \"NEWRELIC_LICENSE_KEY\" was not provided.");
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.NewRelicLogs(
                endpointUrl: "https://log-api.newrelic.com/log/v1",
                applicationName: "locadora-de-veiculos-api",
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

        services.AddValidatorsFromAssemblyContaining<EmployeeValidators>();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<CreateEmployeeRequest>();
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
        string? hangfireConnectionString = configuration["HANGFIRE_SQL_CONNECTION_STRING"];

        if (string.IsNullOrWhiteSpace(hangfireConnectionString))
        {
            throw new Exception("The environment variable \"HANGFIRE_SQL_CONNECTION_STRING\" was not provided.");
        }

        services.AddHangfire(config =>
            config.UseSqlServerStorage(hangfireConnectionString));

        services.AddHangfireServer();
    }

    private static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        string? redisConnectionString = configuration["REDIS_CONNECTION_STRING"];

        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            throw new Exception("The environment variable \"REDIS_CONNECTION_STRING\" was not provided.");
        }

        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = redisConnectionString;
            option.InstanceName = "locadoradeautomoveis";
        });
    }
}
