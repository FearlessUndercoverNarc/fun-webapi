using Microsoft.Extensions.DependencyInjection;
using Services.CommonServices.Abstractions;
using Services.CommonServices.Implementations;

namespace Services.CommonServices
{
    public static class DI
    {
        public static IServiceCollection AddFunCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<IRequestCounterService, RequestCounterService>();
            services.AddSingleton<IImageService, ImageService>();
            return services;
        }
    }
}