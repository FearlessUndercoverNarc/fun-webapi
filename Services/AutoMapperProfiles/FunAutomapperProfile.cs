using AutoMapper;
using Models.Db.Account;
using Models.Db.Tree;
using Models.DTOs.Cards;
using Models.DTOs.Desks;
using Models.DTOs.Folders;
using Models.DTOs.FunAccounts;

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
        }
    }
}