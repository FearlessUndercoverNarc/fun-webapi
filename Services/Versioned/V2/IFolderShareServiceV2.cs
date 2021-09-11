using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Relations;

namespace Services.Versioned.V2
{
    public interface IFolderShareServiceV2
    {
        Task Share(long id, long recipientId, bool hasWriteAccess);

        Task<ICollection<FolderShareDto>> GetShares(long id);

        Task RemoveShare(long id, long recipientId);
    }
}