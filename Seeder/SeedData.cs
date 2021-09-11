using System;
using System.Threading.Tasks;
using Infrastructure.Core;
using Microsoft.Extensions.DependencyInjection;
using Models.DTOs.Cards;
using Models.DTOs.Desks;
using Models.DTOs.Folders;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;
using Services.Versioned.V2;

namespace Seeder
{
    public class SeedData
    {
        private readonly IFunAccountService _funAccountService;
        private readonly IFolderServiceV2 _folderService;
        private readonly IDeskServiceV2 _deskService;
        private readonly IDeskShareServiceV2 _deskShareService;
        private readonly ICardServiceV1 _cardService;
        private readonly IRequestAccountIdSetterService _requestAccountIdSetterService;
        private readonly IServiceScope _serviceScope;

        public SeedData(IServiceProvider provider)
        {
            _serviceScope = provider.CreateScope();
            Context = _serviceScope.ServiceProvider.GetRequiredService<FunDbContext>();
            _funAccountService = _serviceScope.ServiceProvider.GetRequiredService<IFunAccountService>();
            _folderService = _serviceScope.ServiceProvider.GetRequiredService<IFolderServiceV2>();
            _deskService = _serviceScope.ServiceProvider.GetRequiredService<IDeskServiceV2>();
            _deskShareService = _serviceScope.ServiceProvider.GetRequiredService<IDeskShareServiceV2>();
            _requestAccountIdSetterService = _serviceScope.ServiceProvider.GetRequiredService<IRequestAccountIdSetterService>();
            _cardService = _serviceScope.ServiceProvider.GetRequiredService<ICardServiceV1>();
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

            long adminId = await _funAccountService.CreateFunAccount(new() {Login = "Admin", Password = "e3afed0047b08059d0fada10f400c1e5", Fio = "Admin Adminovich Adminov"});
            long gpelId = await _funAccountService.CreateFunAccount(new() {Login = "Gpel", Password = "63214eb0e19eae2e0547d7c3891b6146", Fio = "Genych"});
            await _funAccountService.CreateFunAccount(new() {Login = "Egop", Password = "27adc6b116ca4aea87cee80cfb838b9b", Fio = "Egop Egopovich Egopov"});
            Console.WriteLine("Seeded accounts");

            _requestAccountIdSetterService.Set(gpelId, true);
            long folderId = await _folderService.Create(new CreateFolderDto()
            {
                ParentId = null,
                Title = "Test Folder"
            });

            long deskId = await _deskService.Create(new CreateDeskDto()
            {
                ParentId = folderId,
                Title = "Test Desk",
                Description = "There could be your ads."
            });

            var gaygay = "GAYGAY";
            var colors = new[] {"#e40303", "#ff8c00", "#ffed00", "#008026", "#004dff", "#750787"};
            for (var i = 0; i < gaygay.Length; i++)
            {
                await _cardService.Create(new CreateCardDto()
                {
                    X = (uint)(4940 + 20 * i),
                    Y = (uint)(4940 + 20 * i),
                    ColorHex = colors[i],
                    Title = "" + gaygay[i],
                    Description = "" + gaygay[i],
                    DeskId = deskId,
                    Image = "",
                    ExternalUrl = ""
                });
            }

            await _deskShareService.Share(deskId, adminId);
        }
    }
}