using WebUI.Services;
using WebUI.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Configure HttpClient for EmployeeContactService with optional settings
builder.Services.AddHttpClient<IEmployeeContactService, EmployeeContactService>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl); // Set BaseAddress from config
});

// Register EmployeeContactService
builder.Services.AddScoped<IEmployeeContactService, EmployeeContactService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/index.html")
    {
        context.Response.Redirect("/");
        return;
    }

    await next();
});

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
