using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Folders;
using Models.DTOs.Misc;

namespace Services.Versioned.V1
{
    public interface IFolderServiceV1
    {
        Task<CreatedDto> Create(CreateFolderDto createFolderDto);

        Task Update(UpdateFolderDto updateFolderDto);

        Task<ICollection<FolderWithIdDto>> GetMyRoot();
        
        Task<ICollection<FolderWithIdDto>> GetSharedToMeRoot();

        Task<ICollection<FolderWithIdDto>> GetMyTrashBin();

        Task<ICollection<FolderWithIdDto>> GetSubfoldersByFolder(long id);

        Task MoveToFolder(long id, long? destinationId);

        Task MoveToTrashBin(long id);

        Task RestoreFromTrashBin(long id);

        Task RemoveFromTrashBin(long id);
    }
}