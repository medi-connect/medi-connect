using DoctorService.Utils;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", builder =>
        builder.WithOrigins("http://localhost:5555") // Replace with your frontend URL(s)
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add services to the container.
Env.Load();

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")));
builder.Services.AddHttpClient();

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("FrontendPolicy");

app.Run();