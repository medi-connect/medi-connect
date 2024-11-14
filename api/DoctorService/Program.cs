using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
Env.Load();

builder.Services.AddControllers();
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();