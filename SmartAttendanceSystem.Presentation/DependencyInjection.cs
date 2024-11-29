# region Usings

using Mapster;
using System.Text;
using MapsterMapper;
using FluentValidation;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SmartAttendanceSystem.Core.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartAttendanceSystem.Infrastructure.Persistence;
using SmartAttendanceSystem.Infrastructure.Authentication;
using SmartAttendanceSystem.Application.ServicesImplementation;
using SmartAttendanceSystem.Infrastructure.Persistence.IdentityEntities;

#endregion

namespace SmartAttendanceSystem.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddDistributedMemoryCache();
        services.AddOpenApi();
        services.AddMapsterConfig();
        services.AddFluentValidationConfig();
        services.AddExceptionHandlerConfig();
        services.AddDbSqlConfig(configuration);
        services.AddCorsConfig(configuration);
        services.AddAuthConfig(configuration);

        services.AddScoped<IAuthService<AuthResponse, RegisterRequest>, AuthService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<ICourseService<Course, CourseResponse, CourseRequest>, CourseService>();

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
        #region OptionLoad

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.Configure<EmailConfirmationSettings>(configuration.GetSection(nameof(EmailConfirmationSettings)));

        #endregion

        services.AddSingleton<IJwtProvider<ApplicationUser>, JwtProvider>();

        #region AddingTheIdentity

        services.AddIdentity<ApplicationUser, IdentityRole>()
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
}
