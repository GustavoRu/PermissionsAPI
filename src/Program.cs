using BackendApi.Data;
using BackendApi.Permissions.Repositories;
using BackendApi.Permissions.Services;
using BackendApi.Permissions.DTOs;
using BackendApi.Permissions.Validators;
using FluentValidation;
using BackendApi.Permissions.Models;
using BackendApi.Permissions.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using BackendApi.MessagingQueue.Interfaces;
using BackendApi.MessagingQueue.Services;
using BackendApi.MessagingQueue.Config;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.Configure<KafkaSetting>(
    builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IKafkaService, KafkaService>();
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Repositories
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options => { 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

// Validators
builder.Services.AddScoped<IValidator<PermissionInsertDto>, PermissionInsertValidator>();
builder.Services.AddScoped<IValidator<PermissionUpdateDto>, PermissionUpdateValidator>();
builder.Services.AddScoped<IValidator<PermissionTypeDto>, PermissionTypeValidator>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Manejar referencias circulares en la serialización JSON
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Checking database connection...");
        await dbContext.Database.CanConnectAsync();
        logger.LogInformation("✅ Database connection successful");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error connecting to the database");
        throw;
    }
}

app.Run();
