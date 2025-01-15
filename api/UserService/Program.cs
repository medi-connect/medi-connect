using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.HealthChecks;
using UserService.Utils;
using Prometheus;
using System.Diagnostics;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", builder =>
        builder.WithOrigins("http://localhost:5555")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
Env.Load();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<UserService.Controllers.UserService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var dbConnString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnString));

builder.Services.AddHealthChecks()
    .AddCheck<DbHealthCheck>("db_health_check", tags: ["db_health_check"]);
builder.Services.AddHealthChecks()
    .AddCheck<LivenessHealthCheck>("liveness_health_check", tags: ["liveness_health_check"]);

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_TOKEN_ISSUER") 
                ?? throw new InvalidOperationException("JWT_TOKEN_ISSUER is not set.");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
                  ?? throw new InvalidOperationException("JWT_AUDIENCE is not set.");
var jwtSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") 
                    ?? throw new InvalidOperationException("JWT_SIGNING_KEY is not set or is empty.");

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey))
    };
});

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseHealthChecks("/health");
app.UseHealthChecks("/health/db", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("db_health_check"),
});
app.UseHealthChecks("/health/liveness", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("liveness_health_check"),
});
// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors("FrontendPolicy");

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

app.Run();