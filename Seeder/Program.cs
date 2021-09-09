using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace Seeder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection().AddFunDependencies().BuildServiceProvider();

            if ((Environment.GetEnvironmentVariable("IS_PRODUCTION") ?? "") == "1")
            {
                Console.WriteLine("Seeding production");
                await new SeedData(serviceProvider).SeedDevelopment();
            }
            else
            {
                Console.WriteLine("Seeding development");
                await new SeedData(serviceProvider).SeedDevelopment();
            }
            Console.WriteLine("Seeded successfully");
        }
    }
}