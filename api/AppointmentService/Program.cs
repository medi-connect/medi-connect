using System.Collections;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using AppointmentService.Utils;
using System.Text.Json.Serialization;
using AppointmentService.HealthChecks;
using DoctorService.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", builder =>
        builder.WithOrigins("http://localhost:5555") // Replace with your frontend URL(s)
            .AllowAnyMethod()
            .AllowAnyHeader());
});

Env.Load();

/*foreach (var kv in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process).Cast<DictionaryEntry>())
{
    builder.Configuration[kv.Key.ToString()] = kv.Value?.ToString();
}*/

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddHealthChecks()
    .AddCheck<DbHealthCheck>("db_health_check", tags: ["db_health_check"]);
builder.Services.AddHealthChecks()
    .AddCheck<LivenessHealthCheck>("liveness_health_check", tags: ["liveness_health_check"]);
var dbConnString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                   ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnString));
builder.Services.AddHttpClient();

builder.Configuration.AddEnvironmentVariables();
var app = builder.Build();

app.UseHealthChecks("/health");
app.UseHealthChecks("/health/db", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("db_health_check"),
});
app.UseHealthChecks("/health/liveness", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("liveness_health_check"),
});
// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("FrontendPolicy");

app.Run();