using Amazon.Runtime;
using Amazon.S3;
using FluentValidation;
using Hangfire;
using LocadoraDeAutomoveis.Application.Auth.Services;
using LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;
using LocadoraDeAutomoveis.Application.Rentals.Services;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Shared.Email;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Infrastructure.Auth;
using LocadoraDeAutomoveis.Infrastructure.BillingPlans;
using LocadoraDeAutomoveis.Infrastructure.Clients;
using LocadoraDeAutomoveis.Infrastructure.Configurations;
using LocadoraDeAutomoveis.Infrastructure.Coupons;
using LocadoraDeAutomoveis.Infrastructure.Drivers;
using LocadoraDeAutomoveis.Infrastructure.Email;
using LocadoraDeAutomoveis.Infrastructure.Employees;
using LocadoraDeAutomoveis.Infrastructure.Groups;
using LocadoraDeAutomoveis.Infrastructure.Partners;
using LocadoraDeAutomoveis.Infrastructure.RentalExtras;
using LocadoraDeAutomoveis.Infrastructure.Rentals;
using LocadoraDeAutomoveis.Infrastructure.S3;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using LocadoraDeAutomoveis.Infrastructure.Vehicles;
using LocadoraDeAutomoveis.WebApi.Filters;
using LocadoraDeAutomoveis.WebApi.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        });
    }

    public static void ConfigureOpenApiAuthHeaders(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Locadora de Automoveis API", Version = "v1" });

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

            options.OperationFilter<TenantOverrideHeaderOperationFilter>();
        });
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryRefreshToken, RepositoryRefreshToken>();
        services.AddScoped<IRepositoryEmployee, EmployeeRepository>();
        services.AddScoped<IRepositoryGroup, GroupRepository>();
        services.AddScoped<IRepositoryVehicle, VehicleRepository>();
        services.AddScoped<IRepositoryBillingPlan, BillingPlanRepository>();
        services.AddScoped<IRepositoryClient, ClientRepository>();
        services.AddScoped<IRepositoryDriver, DriverRepository>();
        services.AddScoped<IRepositoryRentalExtra, RentalExtraRepository>();
        services.AddScoped<IRepositoryConfiguration, ConfigurationRepository>();
        services.AddScoped<IRepositoryRental, RentalRepository>();
        services.AddScoped<IRepositoryRentalReturn, RentalReturnRepository>();
        services.AddScoped<IRepositoryPartner, PartnerRepository>();
        services.AddScoped<IRepositoryCoupon, CouponRepository>();
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
                applicationName: "locadora-de-automoveis-api",
                licenseKey: licenseKey
            )
            .CreateLogger();

        logging.ClearProviders();

        services.AddLogging(builder => builder.AddSerilog(dispose: true));
    }

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICouponQueryService, CouponQueryService>();
        services.AddScoped<AcceptLanguageResolver>();

        Assembly applicationAssembly = typeof(ApplicationAssemblyReference).Assembly;

        string? luckyPennySoftwareLicenseKey = configuration["LUCKYPENNYSOFTWARE_LICENSE_KEY"];

        if (string.IsNullOrWhiteSpace(luckyPennySoftwareLicenseKey))
        {
            throw new Exception("The environment variable LUCKYPENNYSOFTWARE_LICENSE_KEY was not provided.");
        }

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
            config.LicenseKey = luckyPennySoftwareLicenseKey;
        });

        services.AddAutoMapper(config =>
            config.LicenseKey = luckyPennySoftwareLicenseKey,
            applicationAssembly
        );

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.ConfigureRecaptchaService();
        services.ConfigureEmailSender(configuration);
        services.ConfigureHangFire(configuration);
        services.ConfigureRedisCache(configuration);
    }

    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ResponseWrapperFilter>();
        }
        ).AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
        ).ConfigureApiBehaviorOptions(options =>
            options.InvalidModelStateResponseFactory = context =>
            {
                string[] errorMessages = context.ModelState
                .Where(entry => entry.Value is { Errors.Count: > 0 })
                .SelectMany(entry => entry.Value!.Errors)
                .Select(error => error.ErrorMessage)
                .ToArray();

                return new JsonResult(errorMessages)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        );
    }

    public static IServiceCollection AddInfrastructureS3(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCloudflareR2Config(configuration);

        services.AddSingleton<IAmazonS3>(sp =>
        {
            CloudflareR2Options options = sp.GetRequiredService<IOptions<CloudflareR2Options>>().Value;

            BasicAWSCredentials credentials = new(options.AccessKeyId, options.SecretAccessKey);

            return new AmazonS3Client(credentials, new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl
            });
        });

        services.AddScoped<R2FileStorageService>();

        return services;
    }

    private static IServiceCollection AddCloudflareR2Config(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CloudflareR2Options>(configuration.GetSection("CLOUDFLARE_R2_CREDENTIALS"));

        return services;
    }

    private static void ConfigureRecaptchaService(this IServiceCollection services)
    {
        services.AddScoped<IRecaptchaService, RecaptchaService>();
        services.AddHttpClient<IRecaptchaService, RecaptchaService>();
    }

    private static void ConfigureEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppUrlsOptions>(configuration.GetSection("APPURLS"));
        services.Configure<MailSettings>(configuration.GetSection("MAILOPTIONS"));
        services.Configure<EmailTemplateOptions>(opt =>
        {
            opt.TemplatesFolderName = "Templates";
            opt.DefaultLanguage = "en-US";
            opt.EnableNeutralLanguageFallback = true;
            opt.HtmlEncodeValues = true;
        });
        services.AddSingleton<IEmailTemplateService, HtmlTemplateService>();
        services.AddTransient<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IAuthEmailService, AuthEmailService>();
        services.AddScoped<IRentalEmailService, RentalEmailService>();
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
