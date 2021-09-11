using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.DTOs;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class DeskActionService : IDeskActionServiceV1
    {
        private IDeskActionHistoryRepository _deskActionHistoryRepository;
        private IMapper _mapper;

        public DeskActionService(IDeskActionHistoryRepository deskActionHistoryRepository, IMapper mapper)
        {
            _deskActionHistoryRepository = deskActionHistoryRepository;
            _mapper = mapper;
        }

        async Task<DeskActionDto> IDeskActionServiceV1.GetById(long id)
        {
            var deskActionHistoryItem = await _deskActionHistoryRepository.GetById(id);

            var deskActionDto = _mapper.Map<DeskActionDto>(deskActionHistoryItem);

            return deskActionDto;
        }

        async Task<ICollection<DeskActionDto>> IDeskActionServiceV1.GetAllByDesk(long id)
        {
            var deskActionHistoryItems = await _deskActionHistoryRepository.GetMany(i => i.DeskId == id);

            var deskActionDtos = _mapper.Map<ICollection<DeskActionDto>>(deskActionHistoryItems);

            return deskActionDtos;
        }
    }
}