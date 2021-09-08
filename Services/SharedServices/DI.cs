using Microsoft.Extensions.DependencyInjection;
using Services.SharedServices.Abstractions;
using Services.SharedServices.Implementations;

namespace Services.SharedServices
{
    public static class DI
    {
        public static IServiceCollection AddFunSharedServices(this IServiceCollection services)
        {
            services.AddScoped<RequestAccountIdService>();
            services.AddScoped<IRequestAccountIdService, RequestAccountIdService>(x => x.GetRequiredService<RequestAccountIdService>());
            services.AddScoped<IRequestAccountIdSetterService, RequestAccountIdService>(x => x.GetRequiredService<RequestAccountIdService>());
            
            services.AddScoped<ITokenSessionService, TokenSessionService>();
            return services;
        }
    }
}