using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs;

namespace Services.Versioned.V2
{
    public interface IDeskActionServiceV2
    {
        Task<DeskActionDto> GetById(long id);
        
        Task<ICollection<DeskActionDto>> GetAllByDesk(long id);
    }
}