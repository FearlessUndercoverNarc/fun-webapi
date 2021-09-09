using System;
using System.Threading.Tasks;
using Infrastructure.Core;
using Microsoft.Extensions.DependencyInjection;
using Services.SharedServices.Abstractions;

namespace Seeder
{
    public class SeedData
    {
        private IFunAccountService _funAccountService;
        private readonly IServiceScope _serviceScope;

        public SeedData(IServiceProvider provider)
        {
            _serviceScope = provider.CreateScope();
            Context = _serviceScope.ServiceProvider.GetRequiredService<FunDbContext>();
            _funAccountService = _serviceScope.ServiceProvider.GetRequiredService<IFunAccountService>();
        }

        ~SeedData()
        {
            _serviceScope.Dispose();
        }

        private FunDbContext Context { get; set; }

        public async Task SeedDevelopment()
        {
            await Context.Database.EnsureDeletedAsync();
            await Context.Database.EnsureCreatedAsync();

            Console.WriteLine("Database dropped and recreated");

            await _funAccountService.CreateFunAccount(new() {Login = "Admin", Password = "e3afed0047b08059d0fada10f400c1e5", Fio = "Admin Adminovich Adminov"});
            await _funAccountService.CreateFunAccount(new() {Login = "Egop", Password = "Egop", Fio = "Egop Egopovich Egopov"});
            Console.WriteLine("Seeded accounts");
        }
    }
}