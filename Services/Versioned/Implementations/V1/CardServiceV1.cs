using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.Cards;
using Models.DTOs.Misc;
using Models.Misc;
using Newtonsoft.Json;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class CardService : ICardServiceV1
    {
        private ICardRepository _cardRepository;

        private IRequestAccountIdService _requestAccountIdService;

        private IDeskRepository _deskRepository;

        private IMapper _mapper;
        private IDeskShareRepository _deskShareRepository;

        private IDeskActionHistoryRepository _deskActionHistoryRepository;
        private ISSEService _sseService;

        public CardService(ICardRepository cardRepository, IMapper mapper, IRequestAccountIdService requestAccountIdService, IDeskRepository deskRepository, IDeskShareRepository deskShareRepository, IDeskActionHistoryRepository deskActionHistoryRepository, ISSEService sseService)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
            _requestAccountIdService = requestAccountIdService;
            _deskRepository = deskRepository;
            _deskShareRepository = deskShareRepository;
            _deskActionHistoryRepository = deskActionHistoryRepository;
            _sseService = sseService;
        }

        async Task<CreatedDto> ICardServiceV1.Create(CreateCardDto createCardDto)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var desk = await _deskRepository.GetById(createCardDto.DeskId);

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV1.Create:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к изменению этой доски");
            }

            var count = await _cardRepository.Count(c => c.DeskId == desk.Id);

            if (count >= 30)
            {
                throw new FunException("Вы не можете создавать более 30 карточек на одной доске. Оформите подписку, чтобы увеличить лимит.");
            }

            var card = _mapper.Map<Card>(createCardDto);

            await _cardRepository.Add(card);

            var lastVersionByDesk = await _deskActionHistoryRepository.GetLastVersionByDesk(desk.Id);

            var deskActionHistoryItem = new DeskActionHistoryItem()
            {
                DeskId = desk.Id,
                DateTime = DateTime.Now,
                FunAccountId = requestAccountId,
                Version = lastVersionByDesk + 1,
                Action = ActionType.CreateCard,
                OldData = "",
                NewData = JsonConvert.SerializeObject(new object[] {card.Id, card.X, card.Y, card.Title, card.Image, card.Description, card.ExternalUrl, card.ColorHex})
            };

            await _deskActionHistoryRepository.Add(deskActionHistoryItem);

            _sseService.EmitDeskActionOccured(desk.Id, deskActionHistoryItem.Id);

            return card.Id;
        }

        async Task ICardServiceV1.Update(UpdateCardDto updateCardDto)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                updateCardDto.Id,
                c => c.Desk
            );

            var desk = card.Desk;

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV1.Update:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к изменению этой доски");
            }

            var oldData = JsonConvert.SerializeObject(new object[] {card.Id, card.X, card.Y, card.Title, card.Image, card.Description, card.ExternalUrl, card.ColorHex});

            _mapper.Map(updateCardDto, card);

            await _cardRepository.Update(card);

            var lastVersionByDesk = await _deskActionHistoryRepository.GetLastVersionByDesk(desk.Id);

            var deskActionHistoryItem = new DeskActionHistoryItem()
            {
                DeskId = desk.Id,
                DateTime = DateTime.Now,
                FunAccountId = requestAccountId,
                Version = lastVersionByDesk + 1,
                Action = ActionType.UpdateCard,
                OldData = oldData,
                NewData = JsonConvert.SerializeObject(new object[] {card.Id, card.X, card.Y, card.Title, card.Image, card.Description, card.ExternalUrl, card.ColorHex})
            };

            await _deskActionHistoryRepository.Add(deskActionHistoryItem);

            _sseService.EmitDeskActionOccured(desk.Id, deskActionHistoryItem.Id);
        }

        async Task<ICollection<CardWithIdDto>> ICardServiceV1.GetAllByDesk(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desk = await _deskRepository.GetById(id);

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV1.GetAllByDesk:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доски");
            }

            var cards = await _cardRepository.GetMany(c => c.DeskId == id);

            var cardWithIdDtos = _mapper.Map<ICollection<CardWithIdDto>>(cards);

            return cardWithIdDtos;
        }

        async Task<ICollection<CardWithIdDto>> ICardServiceV1.GetByDeskAndRect(long id, uint left, uint right, uint top, uint bottom)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desk = await _deskRepository.GetById(id);

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV1.GetByDeskAndRect:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            var cards = await _cardRepository.GetMany(c =>
                c.DeskId == id &&
                c.X >= left && c.X < right &&
                c.Y >= top && c.Y < bottom
            );

            var cardWithIdDtos = _mapper.Map<ICollection<CardWithIdDto>>(cards);

            return cardWithIdDtos;
        }

        async Task<CardWithIdDto> ICardServiceV1.GetById(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                id,
                c => c.Desk
            );

            var desk = card.Desk;

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV1.GetById:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            var cardWithIdDto = _mapper.Map<CardWithIdDto>(card);

            return cardWithIdDto;
        }

        async Task ICardServiceV1.Remove(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                id,
                c => c.Desk
            );

            var desk = card.Desk;

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV1.GetById:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            var oldData = JsonConvert.SerializeObject(new object[] {card.Id, card.X, card.Y, card.Title, card.Image, card.Description, card.ExternalUrl, card.ColorHex});

            await _cardRepository.Remove(card);

            var lastVersionByDesk = await _deskActionHistoryRepository.GetLastVersionByDesk(desk.Id);

            var deskActionHistoryItem = new DeskActionHistoryItem()
            {
                DeskId = desk.Id,
                DateTime = DateTime.Now,
                FunAccountId = requestAccountId,
                Version = lastVersionByDesk + 1,
                Action = ActionType.DeleteCard,
                OldData = oldData,
                NewData = ""
            };

            await _deskActionHistoryRepository.Add(deskActionHistoryItem);

            _sseService.EmitDeskActionOccured(desk.Id, deskActionHistoryItem.Id);
        }
    }
}