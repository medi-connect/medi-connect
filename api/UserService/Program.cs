using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Controllers;
using UserService.Utils;

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
// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors("FrontendPolicy");

app.Run();