using Application.Interfaces.Application;
using Application.Services;
using Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class Extension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDemoService<DemoDTO>, DemoService>();
        }
    }
}