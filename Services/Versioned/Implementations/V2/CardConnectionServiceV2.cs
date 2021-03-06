using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Db.Tree;
using Models.DTOs.CardConnections;
using Models.DTOs.Misc;
using Models.Misc;
using Newtonsoft.Json;
using Services.External;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class CardConnectionService : ICardConnectionServiceV2
    {
        async Task<CreatedDto> ICardConnectionServiceV2.Create(CreateCardConnectionDto createCardConnectionDto)
        {
            if (createCardConnectionDto.CardLeftId == createCardConnectionDto.CardRightId)
            {
                await TelegramAPI.Send($"ICardConnectionServiceV2.Create:\nAttempt to create connection to self!\nCard ({createCardConnectionDto.CardLeftId})");
                throw new FunException("Невозможно соединить карточку с самой собой!");
            }

            var requestAccountId = _requestAccountIdService.Id;

            var cardLeft = await _cardRepository.GetById(createCardConnectionDto.CardLeftId);
            var cardRight = await _cardRepository.GetById(createCardConnectionDto.CardRightId);

            var cardLeftId = cardLeft.Id;
            var cardRightId = cardRight.Id;

            var existingCardConnection = await _cardConnectionRepository.GetOne(c =>
                (c.CardLeftId == cardLeftId && c.CardRightId == cardRightId) ||
                (c.CardLeftId == cardRightId && c.CardRightId == cardLeftId)
            );

            if (existingCardConnection != null)
            {
                await TelegramAPI.Send($"ICardConnectionServiceV2.Create:\nAttempt to create connection between already connected cards!\nCardLeft ({cardLeftId}), CardRight ({cardRightId})");
                throw new FunException("Эти карточки уже соединены!");
            }

            if (cardLeft.DeskId != cardRight.DeskId)
            {
                await TelegramAPI.Send($"ICardConnectionServiceV2.Create:\nAttempt to create connection between cards in different desks!\nCardLeft ({cardLeftId}), CardRight ({cardRightId})");
                throw new FunException("Невозможно соединить эти карточки, так как они находятся на разных досках.");
            }

            var desk = await _deskRepository.GetById(cardLeft.DeskId);

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardConnectionServiceV2.Create:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            var cardConnection = _mapper.Map<CardConnection>(createCardConnectionDto);

            await _cardConnectionRepository.Add(cardConnection);

            var lastVersionByDesk = await _deskActionHistoryRepository.GetLastVersionByDesk(desk.Id);

            var deskActionHistoryItem = new DeskActionHistoryItem()
            {
                DeskId = desk.Id,
                DateTime = DateTime.Now,
                FunAccountId = requestAccountId,
                Version = lastVersionByDesk + 1,
                Action = ActionType.Connect,
                OldData = "",
                NewData = JsonConvert.SerializeObject(new object[] {cardConnection.CardLeftId, cardConnection.CardRightId})
            };

            await _deskActionHistoryRepository.Add(deskActionHistoryItem);

            _sseService.EmitDeskActionOccured(desk.Id, deskActionHistoryItem.Id);

            return cardConnection.Id;
        }

        async Task ICardConnectionServiceV2.Remove(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var cardConnection = await _cardConnectionRepository.GetById(id);

            if (cardConnection == null)
            {
                await TelegramAPI.Send($"CardConnections ({id}) not found");
                throw new FunException($"CardConnections ({id}) not found");
            }

            var card = await _cardRepository.GetById(cardConnection.CardLeftId, c => c.Desk);

            var desk = card.Desk;
            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardConnectionServiceV2.Remove:\nAttempt to access restricted desk!\nDesk ({card.DeskId}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            await _cardConnectionRepository.Remove(cardConnection);

            var lastVersionByDesk = await _deskActionHistoryRepository.GetLastVersionByDesk(desk.Id);

            var deskActionHistoryItem = new DeskActionHistoryItem()
            {
                DeskId = desk.Id,
                DateTime = DateTime.Now,
                FunAccountId = requestAccountId,
                Version = lastVersionByDesk + 1,
                Action = ActionType.Disconnect,
                OldData = "",
                NewData = JsonConvert.SerializeObject(new object[] {cardConnection.CardLeftId, cardConnection.CardRightId})
            };

            await _deskActionHistoryRepository.Add(deskActionHistoryItem);

            _sseService.EmitDeskActionOccured(desk.Id, deskActionHistoryItem.Id);
        }

        async Task<ICollection<CardConnectionWithIdDto>> ICardConnectionServiceV2.GetAllByDesk(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desk = await _deskRepository.GetById(id);

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.HasSharedReadTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"ICardConnectionServiceV2.GetAllByDesk:\nAttempt to access restricted desk!\nDesk ({id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            var cardConnections = await _cardConnectionRepository.GetMany(c => c.CardLeft.DeskId == id);

            var cardConnectionWithIdDtos = _mapper.Map<ICollection<CardConnectionWithIdDto>>(cardConnections);

            return cardConnectionWithIdDtos;
        }
    }
}