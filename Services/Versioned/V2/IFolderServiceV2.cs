using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Folders;
using Models.DTOs.Misc;

namespace Services.Versioned.V2
{
    public interface IFolderServiceV2
    {
        Task<CreatedDto> CreateV2(CreateFolderDto createFolderDto);
        
        Task UpdateV2(UpdateFolderDto updateFolderDto);
        
        Task<ICollection<FolderWithIdDto>> GetMyRootV2();
        
        Task<ICollection<FolderWithIdDto>> GetMyTrashBinV2();
        
        Task<ICollection<FolderWithIdDto>> GetSubfoldersByFolderV2(long folderId);

        Task MoveToTrashV2(long folderId);
        
        Task RestoreFromTrashV2(long folderId);
    }
}