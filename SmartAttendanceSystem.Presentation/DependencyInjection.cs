# region Usings

using SmartAttendanceSystem.Presentation.OpenApiTransformers;
using SmartAttendanceSystem.Infrastructure.Authentication;
using SmartAttendanceSystem.Infrastructure.Repositories;
using SmartAttendanceSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartAttendanceSystem.Infrastructure.Helpers;
using SmartAttendanceSystem.Infrastructure.Health;
using SmartAttendanceSystem.Application.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using SmartAttendanceSystem.Fingerprint;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using System.Reflection;
using FluentValidation;
using MapsterMapper;
using System.Text;
using Asp.Versioning.ApiExplorer;

#endregion

namespace SmartAttendanceSystem.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddDistributedMemoryCache();
        services.AddMapsterConfig();
        services.AddFluentValidationConfig();
        services.AddExceptionHandlerConfig();
        services.AddOptionsLoadConfig(configuration);
        services.AddDbSqlConfig(configuration);
        services.AddCorsConfig(configuration);
        services.AddAuthConfig(configuration);
        services.AddHangfireConfig(configuration);
        services.AddHealthCheckConfig(configuration);
        services.AddVersioningConfig();
        services.AddRateLimiter();
        services.AddFingerprint();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IRoleAskService, RoleAskService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddHttpContextAccessor();

        services
            .AddEndpointsApiExplorer()
            .AddOpenApiConfig();

        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var MappingConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Scan(Assembly.Load("SmartAttendanceSystem.Application"));

        services.AddSingleton<IMapper>(new Mapper(MappingConfig));

        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.Load("SmartAttendanceSystem.Application"));

        return services;
    }

    private static IServiceCollection AddDbSqlConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("The ConnectionString is not found");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseLazyLoadingProxies().UseSqlServer(connectionString)
        );

        services.AddScoped<IDbContextManager, ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddExceptionHandlerConfig(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
            )
        );

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        #region Jwt

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        #endregion

        #region Roles

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        #endregion

        #region AddingTheIdentity

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        #endregion

        #region Validations

        var settings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings?.Key!)),
                    ValidIssuer = settings?.Issuer,
                    ValidAudience = settings?.Audience
                };
            });

        #endregion

        #region IdentityConfigurations

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        #endregion

        return services;
    }

    private static IServiceCollection AddHangfireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJobManager, JobManager>();

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddOpenApiConfig(this IServiceCollection services)
    {
        //Doc -> https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0&tabs=visual-studio

        var serviceProvider = services.BuildServiceProvider();
        var apiVersionDescriptionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                options.AddDocumentTransformer(new ApiVersioningTransformer(description));
            });
        }
        return services;
    }

    private static IServiceCollection AddOptionsLoadConfig(this IServiceCollection services, IConfiguration configuration)
    {
        #region Mail

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.Configure<EmailConfirmationSettings>(configuration.GetSection(nameof(EmailConfirmationSettings)));

        #endregion

        #region Fingerprint

        services.Configure<EnrollmentCommands>(configuration.GetSection(nameof(EnrollmentCommands)));

        #endregion

        #region InstructorRole

        services.AddOptions<InstructorRoleSettings>()
            .BindConfiguration(InstructorRoleSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        #endregion

        return services;
    }
    
    private static IServiceCollection AddHealthCheckConfig(this IServiceCollection services, IConfiguration configuration)
    {
        //For Url we can add httpMethod

        services.AddHealthChecks()
            .AddSqlServer(name: "Database", connectionString: configuration.GetConnectionString("DefaultConnection")!, tags: ["Db"])
            .AddHangfire(options => { options.MinimumAvailableServers = 1; })
            .AddUrlGroup(name: "Network", uri: new Uri("https://www.google.com"))
            .AddCheck<MailProviderHealthCheck>(name: "Mail service");

        return services;
    }

    private static IServiceCollection AddRateLimiter(this IServiceCollection services)
    {
        //[EnableRateLimiting("")]

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.AddPolicy(RateLimiterSettings.IpLimit, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(15)
                    }
                )
            );
            
            rateLimiterOptions.AddPolicy(RateLimiterSettings.UserLimit, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.GetId(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(15)
                    }
                )
            );

            rateLimiterOptions.AddConcurrencyLimiter(RateLimiterSettings.Concurrency, options =>
            {
                options.PermitLimit = 1000;
                options.QueueLimit = 100;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            rateLimiterOptions.AddFixedWindowLimiter(RateLimiterSettings.Fixed, options =>
            {
                options.PermitLimit = 20;
                options.QueueLimit = 5;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

                options.Window = TimeSpan.FromSeconds(15);
            });

            rateLimiterOptions.AddTokenBucketLimiter(RateLimiterSettings.Token, options =>
            {
                options.QueueLimit = 10;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

                options.TokenLimit = 50;
                options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                options.TokensPerPeriod = 5;
                options.AutoReplenishment = true;
            });

            rateLimiterOptions.AddSlidingWindowLimiter(RateLimiterSettings.Sliding, options =>
            {
                options.PermitLimit = 20;
                options.QueueLimit = 5;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

                options.Window = TimeSpan.FromSeconds(30);
                options.SegmentsPerWindow = 3;
            });
        });

        return services;
    }

    private static IServiceCollection AddVersioningConfig(this IServiceCollection services)
    {
        //In the head of controller we can add the version [ApiVersion(1, Deprecated = true)]
        //In the head of endpoint we can add the version [MapToApiVersion(1)]

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;

            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        }
        ).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}
