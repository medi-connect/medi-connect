using System.Reflection;
using DotNetEnv;
using FeedbackService.HealthChecks;
using FeedbackService.Utils;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", builder =>
        builder.WithOrigins("http://localhost:5555") // Replace with your frontend URL(s)
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddHealthChecks()
    .AddCheck<DbHealthCheck>("db_health_check", tags: ["db_health_check"]);
builder.Services.AddHealthChecks()
    .AddCheck<LivenessHealthCheck>("liveness_health_check", tags: ["liveness_health_check"]);
// gRPC
builder.Services.AddGrpc();
var dbConnString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                   ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnString));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddHttpClient();
builder.Configuration.AddEnvironmentVariables();


var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();
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

app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();