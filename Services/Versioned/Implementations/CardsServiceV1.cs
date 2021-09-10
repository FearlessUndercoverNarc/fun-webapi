using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.Cards;
using Models.DTOs.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class CardsService : ICardsServiceV1
    {
        private ICardRepository _cardRepository;

        private IRequestAccountIdService _requestAccountIdService;

        private IDeskRepository _deskRepository;

        private IMapper _mapper;

        public CardsService(ICardRepository cardRepository, IMapper mapper, IRequestAccountIdService requestAccountIdService, IDeskRepository deskRepository)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
            _requestAccountIdService = requestAccountIdService;
            _deskRepository = deskRepository;
        }

        async Task<CreatedDto> ICardsServiceV1.Create(CreateCardDto createCardDto)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var desk = await _deskRepository.GetById(createCardDto.DeskId);

            // TODO: Support shared desks
            if (requestAccountId != desk.AuthorAccountId)
            {
                await TelegramAPI.Send($"ICardsServiceV1.Create:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new("У вас нет доступа к изменению этой доски");
            }

            var card = _mapper.Map<Card>(createCardDto);

            await _cardRepository.Add(card);

            // TODO: Raise SSE event

            return card.Id;
        }

        async Task ICardsServiceV1.Update(UpdateCardDto updateCardDto)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                updateCardDto.Id,
                c => c.Desk
            );

            var desk = card.Desk;

            // TODO: Support shared desks
            if (requestAccountId != desk.AuthorAccountId)
            {
                await TelegramAPI.Send($"ICardsServiceV1.Update:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new("У вас нет доступа к изменению этой доски");
            }

            _mapper.Map(updateCardDto, card);

            await _cardRepository.Update(card);

            // TODO: Raise SSE event
        }

        async Task<ICollection<CardWithIdDto>> ICardsServiceV1.GetAllByDesk(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desk = await _deskRepository.GetById(id);

            // TODO: Support shared desks
            if (requestAccountId != desk.AuthorAccountId)
            {
                await TelegramAPI.Send($"ICardsServiceV1.GetAllByDesk:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new("У вас нет доступа к этой доски");
            }

            var cards = await _cardRepository.GetMany(c => c.DeskId == id);

            var cardWithIdDtos = _mapper.Map<ICollection<CardWithIdDto>>(cards);

            return cardWithIdDtos;
        }

        async Task<ICollection<CardWithIdDto>> ICardsServiceV1.GetByDeskAndRect(long id, uint left, uint right, uint top, uint bottom)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desk = await _deskRepository.GetById(id);

            // TODO: Support shared desks
            if (requestAccountId != desk.AuthorAccountId)
            {
                await TelegramAPI.Send($"ICardsServiceV1.GetByDeskAndRect:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new("У вас нет доступа к этой доске");
            }

            var cards = await _cardRepository.GetMany(c =>
                c.DeskId == id &&
                c.X >= left && c.X < right &&
                c.Y >= top && c.Y < bottom
            );

            var cardWithIdDtos = _mapper.Map<ICollection<CardWithIdDto>>(cards);

            return cardWithIdDtos;
        }

        async Task<CardWithIdDto> ICardsServiceV1.GetById(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                id,
                c => c.Desk
            );

            var desk = card.Desk;

            // TODO: Support shared desks
            if (requestAccountId != desk.AuthorAccountId)
            {
                await TelegramAPI.Send($"ICardsServiceV1.GetById:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new("У вас нет доступа к этой доске");
            }

            var cardWithIdDto = _mapper.Map<CardWithIdDto>(card);

            return cardWithIdDto;
        }

        async Task ICardsServiceV1.Remove(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                id,
                c => c.Desk
            );

            var desk = card.Desk;

            // TODO: Support shared desks
            if (requestAccountId != desk.AuthorAccountId)
            {
                await TelegramAPI.Send($"ICardsServiceV1.GetById:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new("У вас нет доступа к этой доске");
            }
            
            await _cardRepository.Remove(card);

            // TODO: Raise SSE event
        }
    }
}