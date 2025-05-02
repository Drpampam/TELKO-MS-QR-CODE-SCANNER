using Application.Extensions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Confgurations;
using Persistence.Extensions.Persistence;
using Persistence.Extensions.Repository;
using Serilog;
using Persistence.Repositories;
using Application.Features.Configurations;
using Application.Features.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

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
    options.AddDefaultPolicy(builder =>
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

