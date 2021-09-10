using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.CardConnections;
using Models.DTOs.Misc;

namespace Services.Versioned.V2
{
    public interface ICardConnectionServiceV2
    {
        Task<CreatedDto> Create(CreateCardConnectionDto createCardConnectionDto);

        Task Remove(long id);

        Task<ICollection<CardConnectionWithIdDto>> GetAllByDesk(long id);
    }
}