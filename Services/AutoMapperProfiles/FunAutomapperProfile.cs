using AutoMapper;
using Models.Db.Account;
using Models.Db.Relations;
using Models.Db.Tree;
using Models.DTOs;
using Models.DTOs.CardConnections;
using Models.DTOs.Cards;
using Models.DTOs.Desks;
using Models.DTOs.Folders;
using Models.DTOs.FunAccounts;
using Models.DTOs.Relations;
using Models.ImportExport;

namespace Services.AutoMapperProfiles
{
    // --------------------------------------------------------- //
    // EVEN IF YOUR IDE SAYS THIS CODE IS UNUSED, DONT DELETE IT //
    // --------------------------------------------------------- //

    public class FunAutomapperProfile : Profile
    {
        public FunAutomapperProfile()
        {
            // ReverseMap() нужен для обратной конвертации любого мапа

            // CreateMap<string, TimeSpan>().ConvertUsing(s => TimeSpan.ParseExact(s, "hh\\:mm", null));
            // CreateMap<TimeSpan, string>().ConvertUsing(time => $"{time.Hours:00}:{time.Minutes:00}");

            // -----------
            // This doesn't work for some reason, need to specify every derived type
            // CreateMap<LatLngDto, LatLng>().ReverseMap();
            // -----------


            CreateMap<FunAccount, FunAccountWithIdDto>()
                .ReverseMap();
            CreateMap<FunAccount, CreateFunAccountDto>()
                .ReverseMap();
            CreateMap<FunAccount, UpdateFunAccountDto>()
                .ReverseMap();

            CreateMap<Folder, FolderWithIdDto>()
                .ReverseMap();
            CreateMap<Folder, CreateFolderDto>()
                .ReverseMap();
            CreateMap<Folder, UpdateFolderDto>()
                .ReverseMap();

            CreateMap<Desk, DeskWithIdDto>()
                .ForMember(dto => dto.ParentTitle, cfg => cfg.MapFrom(d => d.Parent.Title))
                .ReverseMap();
            CreateMap<Desk, CreateDeskDto>()
                .ReverseMap();
            CreateMap<Desk, UpdateDeskDto>()
                .ReverseMap();

            CreateMap<Card, CardWithIdDto>()
                .ReverseMap();
            CreateMap<Card, CreateCardDto>()
                .ReverseMap();
            CreateMap<Card, UpdateCardDto>()
                .ReverseMap();

            CreateMap<CardConnection, CardConnectionWithIdDto>()
                .ReverseMap();
            CreateMap<CardConnection, CreateCardConnectionDto>()
                .ReverseMap();

            CreateMap<FolderShare, FolderShareDto>()
                .ReverseMap();
            CreateMap<DeskShare, DeskShareDto>()
                .ReverseMap();

            CreateMap<DeskActionHistoryItem, DeskActionDto>()
                .ReverseMap();


            CreateMap<Folder, FolderModel>()
                .ForMember(m => m.Desks, cfg => cfg.Ignore())
                .ForMember(m => m.Children, cfg => cfg.Ignore())
                .ReverseMap()
                .ForMember(f => f.Desks, cfg => cfg.Ignore())
                .ForMember(m => m.Children, cfg => cfg.Ignore());
            CreateMap<Desk, DeskModel>()
                .ForMember(m => m.Cards, cfg => cfg.Ignore())
                .ReverseMap()
                .ForMember(d => d.Cards, cfg => cfg.Ignore());
            CreateMap<Card, CardModel>()
                .ReverseMap();
        }
    }
}