using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Db.Tree;
using Models.DTOs.Cards;
using Models.DTOs.Misc;
using Models.Misc;
using Services.External;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class CardService : ICardServiceV2
    {
        async Task<CreatedDto> ICardServiceV2.Create(CreateCardDto createCardDto)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var desk = await _deskRepository.GetById(createCardDto.DeskId);

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV2.Create:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к изменению этой доски");
            }

            var card = _mapper.Map<Card>(createCardDto);

            await _cardRepository.Add(card);

            // TODO: Raise SSE event

            return card.Id;
        }

        async Task ICardServiceV2.Update(UpdateCardDto updateCardDto)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                updateCardDto.Id,
                c => c.Desk
            );

            var desk = card.Desk;

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV2.Update:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к изменению этой доски");
            }

            _mapper.Map(updateCardDto, card);

            await _cardRepository.Update(card);

            // TODO: Raise SSE event
        }

        async Task<ICollection<CardWithIdDto>> ICardServiceV2.GetAllByDesk(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desk = await _deskRepository.GetById(id);

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV2.GetAllByDesk:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доски");
            }

            var cards = await _cardRepository.GetMany(c => c.DeskId == id);

            var cardWithIdDtos = _mapper.Map<ICollection<CardWithIdDto>>(cards);

            return cardWithIdDtos;
        }

        async Task<ICollection<CardWithIdDto>> ICardServiceV2.GetByDeskAndRect(long id, uint left, uint right, uint top, uint bottom)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desk = await _deskRepository.GetById(id);

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV2.GetByDeskAndRect:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
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

        async Task<CardWithIdDto> ICardServiceV2.GetById(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                id,
                c => c.Desk
            );

            var desk = card.Desk;

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV2.GetById:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            var cardWithIdDto = _mapper.Map<CardWithIdDto>(card);

            return cardWithIdDto;
        }

        async Task ICardServiceV2.Remove(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var card = await _cardRepository.GetById(
                id,
                c => c.Desk
            );

            var desk = card.Desk;

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardServiceV2.GetById:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }
            
            await _cardRepository.Remove(card);

            // TODO: Raise SSE event
        }
    }
}