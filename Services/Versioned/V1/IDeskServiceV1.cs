using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Desks;
using Models.DTOs.Misc;

namespace Services.Versioned.V1
{
    public interface IDeskServiceV1
    {
        Task<CreatedDto> Create(CreateDeskDto createDeskDto);

        Task Update(UpdateDeskDto updateDeskDto);

        Task<DeskWithIdDto> GetById(long id);
        
        Task<ICollection<DeskWithIdDto>> GetByFolder(long folderId);
        
        Task<ICollection<DeskWithIdDto>> GetSharedToMe();
        
        Task MoveToFolder(long deskId, long destinationId);
    }
}