using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using UserService.Controllers;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")));

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();