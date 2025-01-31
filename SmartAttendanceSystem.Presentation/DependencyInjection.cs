# region Usings

using SmartAttendanceSystem.Presentation.OpenApiTransformers;
using SmartAttendanceSystem.Infrastructure.Authentication;
using SmartAttendanceSystem.Infrastructure.Repositories;
using SmartAttendanceSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartAttendanceSystem.Infrastructure.Helpers;
using SmartAttendanceSystem.Application.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using SmartAttendanceSystem.Fingerprint;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using System.Reflection;
using FluentValidation;
using MapsterMapper;
using System.Text;

#endregion

namespace SmartAttendanceSystem.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddDistributedMemoryCache();
        services.AddOpenApiConfig();
        services.AddMapsterConfig();
        services.AddFluentValidationConfig();
        services.AddExceptionHandlerConfig();
        services.AddOptionsLoadConfig(configuration);
        services.AddDbSqlConfig(configuration);
        services.AddCorsConfig(configuration);
        services.AddAuthConfig(configuration);
        services.AddHangfireConfig(configuration);
        services.AddFingerprint();

        services.AddScoped<IDbContextHelper, DbContextHelper>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IUserService, UserService>();

        services.AddHttpContextAccessor();

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
        services.AddSingleton<IJwtProvider, JwtProvider>();

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
        services.AddScoped<IJobScheduler, JobScheduler>();

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
        services
            .AddEndpointsApiExplorer()
            .AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

        return services;
    }

    private static IServiceCollection AddOptionsLoadConfig(this IServiceCollection services, IConfiguration configuration)
    {
        #region Jwt

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        #endregion

        #region Mail

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.Configure<EmailConfirmationSettings>(configuration.GetSection(nameof(EmailConfirmationSettings)));

        #endregion

        #region Fingerprint

        services.Configure<EnrollmentCommands>(configuration.GetSection(nameof(EnrollmentCommands)));

        #endregion

        return services;
    }
}
