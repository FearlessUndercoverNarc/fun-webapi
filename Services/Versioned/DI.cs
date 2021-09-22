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
            services.AddScoped<IFolderTrashBinServiceV1, FolderTrashBinService>();
            services.AddScoped<IFolderTrashBinServiceV2, FolderTrashBinService>();
            services.AddScoped<IDeskServiceV1, DeskService>();
            services.AddScoped<IDeskServiceV2, DeskService>();
            services.AddScoped<IDeskTrashBinServiceV1, DeskTrashBinService>();
            services.AddScoped<IDeskTrashBinServiceV2, DeskTrashBinService>();
            services.AddScoped<ICardServiceV1, CardService>();
            services.AddScoped<ICardServiceV2, CardService>();
            services.AddScoped<ICardConnectionServiceV1, CardConnectionService>();
            services.AddScoped<ICardConnectionServiceV2, CardConnectionService>();
            services.AddScoped<IFolderShareServiceV1, FolderShareService>();
            services.AddScoped<IFolderShareServiceV2, FolderShareService>();
            services.AddScoped<IDeskShareServiceV1, DeskShareService>();
            services.AddScoped<IDeskShareServiceV2, DeskShareService>();
            services.AddScoped<IDeskActionServiceV1, DeskActionService>();
            services.AddScoped<IDeskActionServiceV2, DeskActionService>();
            services.AddScoped<IFolderImportExportServiceV1, FolderImportExportService>();
            services.AddScoped<IFolderImportExportServiceV2, FolderImportExportService>();
            return services;
        }
    }
}