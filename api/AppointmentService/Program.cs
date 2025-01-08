using System.Collections;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using AppointmentService.Utils;
using System.Text.Json.Serialization;

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
//builder.Services.AddDbContext<DbContext>(options =>
//    options.UseSqlServer(builder.Configuration["DB_CONNECTION_STRING"]));
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