using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs;

namespace Services.Versioned.V1
{
    public interface IDeskActionServiceV1
    {
        Task<DeskActionDto> GetById(long id);
        
        Task<ICollection<DeskActionDto>> GetAllByDesk(long id);
    }
}