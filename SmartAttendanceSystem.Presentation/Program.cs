#region Usings

using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using SmartAttendanceSystem.Presentation;

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ],
    DashboardTitle = "SmartAttendanceSystem Dashboard",
    IsReadOnlyFunc = context => true
});

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.UseRateLimiter();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
