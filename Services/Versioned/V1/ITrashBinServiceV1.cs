using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Folders;

namespace Services.Versioned.V1
{
    public interface ITrashBinServiceV1
    {
        Task<ICollection<FolderWithIdDto>> GetMyTrashBin();
        
        Task MoveToTrashBin(long id);

        Task RestoreFromTrashBin(long id);

        Task RemoveFromTrashBin(long id);
    }
}