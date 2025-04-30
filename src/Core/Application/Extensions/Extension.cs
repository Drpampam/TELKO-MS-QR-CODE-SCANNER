using Application.Features.Helpers;
using Application.Interfaces.Application;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class Extension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IHelper, Helper>();
            services.AddScoped<IApplicationService, ApplicationService>();
        }
    }
}