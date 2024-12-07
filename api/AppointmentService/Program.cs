using System.Collections;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

foreach (var kv in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process).Cast<DictionaryEntry>())
{
    builder.Configuration[kv.Key.ToString()] = kv.Value?.ToString();
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(builder.Configuration["DB_CONNECTION_STRING"]));

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();