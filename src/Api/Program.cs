using Application.Extensions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Configurations;
using Persistence.Extensions.Persistence;
using Persistence.Extensions.Repository;
using Serilog;
using Persistence.Repositories;
using Application.Features.Configurations;
using Application.Features.Validations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Setup Serilog configuration early
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // 🔹 Hook Serilog into the host

try
{
    Log.Information("Starting application");

    // 🔹 Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddControllersWithViews();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DataConnectionStrings"));
    });

    builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));

    builder.Services.AddServices();
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddRepository(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    });

    builder.Services.Configure<EmployeeContactDefaultsOptions>(
        builder.Configuration.GetSection("EmployeeContactDefaults"));

    builder.Services.AddScoped<EmployeeContactFormValidator>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSwagger();

    app.UseSwaggerUI();

    app.UseCors();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
