using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Relations;

namespace Services.Versioned.V2
{
    public interface IDeskShareServiceV2
    {
        Task Share(long id, long recipientId, bool hasWriteAccess);

        Task<ICollection<DeskShareDto>> GetShares(long id);

        Task RemoveShare(long id, long recipientId);
    }
}