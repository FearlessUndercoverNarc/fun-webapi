using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Desks;

namespace Services.Versioned.V1
{
    public interface IDeskTrashBinServiceV1
    {
        Task<ICollection<DeskWithIdDto>> GetMyTrashBin();

        Task MoveToTrashBin(long id);
        
        Task RestoreFromTrashBin(long id);
        
        Task RemoveFromTrashBin(long id);
    }
}