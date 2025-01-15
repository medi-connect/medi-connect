using System.Reflection;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using AppointmentService.Utils;
using System.Text.Json.Serialization;
using AppointmentService.GraphQL.Queries;
using AppointmentService.GraphQL.Schema;
using AppointmentService.GraphQL.Types;
using AppointmentService.HealthChecks;
using AppointmentService.Services;
using DoctorService.HealthChecks;
using GraphQL;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using System.Diagnostics;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", builder =>
        builder.WithOrigins("http://72.144.116.77/mediconnect") // Replace with your frontend URL(s)
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

// HEALTH CHECKS
builder.Services.AddHealthChecks()
    .AddCheck<DbHealthCheck>("db_health_check", tags: ["db_health_check"]);
builder.Services.AddHealthChecks()
    .AddCheck<LivenessHealthCheck>("liveness_health_check", tags: ["liveness_health_check"]);

// GraphQL
builder.Services.AddGraphQL(b => b
    .AddSchema<AppointmentSchema>()
    .AddSystemTextJson()
    .AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = true) // Expose stack trace!
    .AddDataLoader());
builder.Services.AddScoped<MainQuery>();
builder.Services.AddSingleton<AppointmentSchema>();
builder.Services.AddSingleton<AppointmentType>();
// gRPC
builder.Services.AddGrpc();
// Database Connection
var dbConnString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                   ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnString));

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(8004, o => o.Protocols = HttpProtocols.Http2); // HTTP/2 without HTTPS
// });

builder.Services.AddHttpClient();

builder.Configuration.AddEnvironmentVariables();
var app = builder.Build();

// Health check routing
app.UseHealthChecks("/health");
app.UseHealthChecks("/health/db", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("db_health_check"),
});
app.UseHealthChecks("/health/liveness", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("liveness_health_check"),
});

// GraphQL routing
app.MapGraphQL();
// gRPC
app.MapGrpcService<DoneAppointmentsServiceImpl>();
// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

var counter = Metrics.CreateCounter("requests_total", "Total number of requests");

app.Use(async (context, next) =>
{
    // Exclude Prometheus metrics endpoint from incrementing the counter
    if (context.Request.Path != "/metrics")
    {
        counter.Inc();
    }
    await next();
});

var httpDuration = Metrics.CreateHistogram("http_duration_seconds", "Histogram of HTTP request durations in seconds", new HistogramConfiguration
{
    LabelNames = new[] { "method", "endpoint" },
    Buckets = Histogram.LinearBuckets(0.1, 0.1, 10)  // Buckets of 0.1s, increasing by 0.1s, up to 10s.
});

app.Use(async (context, next) =>
{
    if (context.Request.Path != "/metrics")
    {
        var stopwatch = Stopwatch.StartNew();
        stopwatch.Stop();

        // Record response duration
        httpDuration.Labels(context.Request.Method, context.Request.Path).Observe(stopwatch.Elapsed.TotalSeconds);   
    }
    await next();
});

var errorCounter = Metrics.CreateCounter("http_errors_total", "Total number of HTTP errors by status code", new CounterConfiguration
{
    LabelNames = new[] { "method", "endpoint", "status_code" }
});

app.Use(async (context, next) =>
{
    await next();
    if (context.Request.Path != "/metrics")
    {
        // Track errors based on the status code
        if (context.Response.StatusCode >= 400)
        {
            errorCounter.Labels(context.Request.Method, context.Request.Path, context.Response.StatusCode.ToString()).Inc();
        }
    }
});

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics(); // Ensure Prometheus metrics endpoint is mapped
});

app.UseCors("FrontendPolicy");

app.Run();