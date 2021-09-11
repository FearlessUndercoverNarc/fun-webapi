using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Desks;
using Models.DTOs.Misc;

namespace Services.Versioned.V2
{
    public interface IDeskServiceV2
    {
        Task<CreatedDto> Create(CreateDeskDto createDeskDto);

        Task Update(UpdateDeskDto updateDeskDto);

        Task<DeskWithIdDto> GetById(long id);

        Task<ICollection<DeskWithIdDto>> GetByFolder(long folderId);
        
        Task<ICollection<DeskWithIdDto>> GetSharedToMe();

        Task MoveToFolder(long deskId, long destinationId);
    }
}