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
using Microsoft.OpenApi.Models;

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

    // 🔹 Configure Swagger with annotations support
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "TelkoMs API", Version = "v1" });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        c.EnableAnnotations();
    });

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

    builder.Services.Configure<EmployeeContactDefaultsOptions>(
        builder.Configuration.GetSection("EmployeeContactDefaults"));
    builder.Services.AddScoped<EmployeeContactFormValidator>();

    var app = builder.Build();

    // 🔹 UsePathBase for IIS virtual directory (e.g., /BarCodeBackEnd)
    if (!app.Environment.IsDevelopment())
    {
        app.UsePathBase("/BarCodeBackEnd");
    }

    // 🔹 Enable static file serving (required for Swagger UI assets)
    app.UseStaticFiles();

    // 🔹 Error handling
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // 🔹 Swagger setup
    Log.Information("It got to swagger");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // 🔹 IMPORTANT: use the correct relative path when hosted in virtual directory
        c.SwaggerEndpoint("/BarCodeBackEnd/swagger/v1/swagger.json", "TelkoMs API v1");
        Log.Information("BarCodeBackEnd/swagger/v1/swagger.json, TelkoMs API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at /BarCodeBackEnd/
    });

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