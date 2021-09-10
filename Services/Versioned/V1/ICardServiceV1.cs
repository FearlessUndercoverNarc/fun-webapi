using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.Cards;
using Models.DTOs.Misc;

namespace Services.Versioned.V1
{
    public interface ICardServiceV1
    {
        Task<CreatedDto> Create(CreateCardDto createCardDto);

        Task Update(UpdateCardDto updateCardDto);

        Task<ICollection<CardWithIdDto>> GetAllByDesk(long id);
        
        Task<ICollection<CardWithIdDto>> GetByDeskAndRect(long id, uint left, uint right, uint top, uint bottom);
        
        Task<CardWithIdDto> GetById(long id);

        Task Remove(long id);
    }
}