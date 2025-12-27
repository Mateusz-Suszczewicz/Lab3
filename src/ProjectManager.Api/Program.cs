using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;
using ProjectManager.Infrastructure;
using ProjectManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectManager API", Version = "v1" });
});
var conn = builder.Configuration.GetConnectionString("Default");

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var retryPolicy = Policy
    .Handle<Exception>()
    .WaitAndRetry(5, retryAttempt =>
        TimeSpan.FromSeconds(3),
        (ex, time) =>
        {
            Console.WriteLine($"DB not ready, retrying in {time.Seconds}s...");
        });

retryPolicy.Execute(() =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
