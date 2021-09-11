using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class DeskActionService : IDeskActionServiceV2
    {
        async Task<DeskActionDto> IDeskActionServiceV2.GetById(long id)
        {
            var deskActionHistoryItem = await _deskActionHistoryRepository.GetById(id);

            var deskActionDto = _mapper.Map<DeskActionDto>(deskActionHistoryItem);

            return deskActionDto;
        }

        async Task<ICollection<DeskActionDto>> IDeskActionServiceV2.GetAllByDesk(long id)
        {
            var deskActionHistoryItems = await _deskActionHistoryRepository.GetMany(i => i.DeskId == id);

            var deskActionDtos = _mapper.Map<ICollection<DeskActionDto>>(deskActionHistoryItems);

            return deskActionDtos;
        }
    }
}