using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Folders;
using Models.DTOs.Misc;

namespace Services.Versioned.V2
{
    public interface IFolderServiceV2
    {
        Task<CreatedDto> Create(CreateFolderDto createFolderDto);

        Task Update(UpdateFolderDto updateFolderDto);

        Task<ICollection<FolderWithIdDto>> GetMyRoot();

        Task<ICollection<FolderWithIdDto>> GetMyTrashBin();

        Task<ICollection<FolderWithIdDto>> GetSubfoldersByFolder(long folderId);

        Task MoveToFolder(long folderId, long? destinationId);

        Task MoveToTrashBin(long folderId);

        Task RestoreFromTrashBin(long folderId);
    }
}