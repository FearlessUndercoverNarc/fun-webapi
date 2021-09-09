using Microsoft.Extensions.DependencyInjection;
using Services.Versioned.Implementations;
using Services.Versioned.V1;
using Services.Versioned.V2;

namespace Services.Versioned
{
    public static class DI
    {
        public static IServiceCollection AddFunVersionedServices(this IServiceCollection services)
        {
            services.AddScoped<IFolderServiceV1, FolderService>();
            services.AddScoped<IFolderServiceV2, FolderService>();
            return services;
        }
    }
}