using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Folders;
using Models.DTOs.Misc;

namespace Services.Versioned.V1
{
    public interface IFolderServiceV1
    {
        Task<CreatedDto> CreateV1(CreateFolderDto createFolderDto);
        
        Task UpdateV1(UpdateFolderDto updateFolderDto);
        
        Task<ICollection<FolderWithIdDto>> GetMyRootV1();
        
        Task<ICollection<FolderWithIdDto>> GetMyTrashBinV1();
        
        Task<ICollection<FolderWithIdDto>> GetSubfoldersByFolderV1(long folderId);

        Task MoveToTrashV1(long folderId);
        
        Task RestoreFromTrashV1(long folderId);
        
    }
}