#region Usings

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SmartAttendanceSystem.Presentation;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Hangfire.Dashboard;
using Scalar.AspNetCore;
using Serilog;

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

#region Hangfire

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
    IsReadOnlyFunc = (DashboardContext context) => true
});

#endregion

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
