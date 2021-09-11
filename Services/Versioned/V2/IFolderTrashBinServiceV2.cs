using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Folders;

namespace Services.Versioned.V2
{
    public interface IFolderTrashBinServiceV2
    {
        Task<ICollection<FolderWithIdDto>> GetMyTrashBin();
        
        Task MoveToTrashBin(long id);

        Task RestoreFromTrashBin(long id);

        Task RemoveFromTrashBin(long id);
    }
}