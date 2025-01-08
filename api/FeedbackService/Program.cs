var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", builder =>
        builder.WithOrigins("http://localhost:5555") // Replace with your frontend URL(s)
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("FrontendPolicy");

app.Run();