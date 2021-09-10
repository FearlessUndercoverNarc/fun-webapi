using System;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Services.AutoMapperProfiles;
using Services.CommonServices;
using Services.SharedServices;
using Services.Versioned;

namespace Services
{
    public static class DI
    {
        public static IServiceCollection AddFunDependencies(this IServiceCollection services)
        {
            // DbContext will take connection string from Environment or throw
            services.AddDbContext<FunDbContext>();

            services.AddFunRepositories();
            
            services.AddFunSharedServices();

            services.AddFunCommonServices();
            services.AddFunVersionedServices();

            services.AddAutoMapper(cfg => cfg.AddProfile<FunAutomapperProfile>());

            services.AddScoped<Func<Type, object, bool>>(x =>
            {
                bool Func(Type t, object o)
                {
                    var dbContext = x.GetRequiredService<FunDbContext>();
                    return dbContext.Find(t, o) != null;
                }

                return Func;
            });

            return services;
        }

        public static IServiceCollection AddFunRepositories(this IServiceCollection services)
        {
            // Add Repositories
            services.AddScoped<ITokenSessionRepository, TokenSessionRepository>();
            services.AddScoped<IFunAccountRepository, FunAccountRepository>();
            services.AddScoped<IFolderRepository, FolderRepository>();
            services.AddScoped<IDeskRepository, DeskRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<ICardConnectionRepository, CardConnectionRepository>();

            return services;
        }
    }
}