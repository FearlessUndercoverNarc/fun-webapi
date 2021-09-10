﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.CardConnections;
using Models.DTOs.Misc;
using Models.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class CardConnectionService : ICardConnectionServiceV1
    {
        private ICardConnectionRepository _cardConnectionRepository;
        private ICardRepository _cardRepository;

        private IDeskRepository _deskRepository;

        private IRequestAccountIdService _requestAccountIdService;

        private IMapper _mapper;

        public CardConnectionService(ICardConnectionRepository cardConnectionRepository, IMapper mapper, IDeskRepository deskRepository, IRequestAccountIdService requestAccountIdService, ICardRepository cardRepository)
        {
            _cardConnectionRepository = cardConnectionRepository;
            _mapper = mapper;
            _deskRepository = deskRepository;
            _requestAccountIdService = requestAccountIdService;
            _cardRepository = cardRepository;
        }

        async Task<CreatedDto> ICardConnectionServiceV1.Create(CreateCardConnectionDto createCardConnectionDto)
        {
            if (createCardConnectionDto.CardLeftId == createCardConnectionDto.CardRightId)
            {
                await TelegramAPI.Send($"ICardConnectionServiceV1.Create:\nAttempt to create connection to self!\nCard ({createCardConnectionDto.CardLeftId})");
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
                await TelegramAPI.Send($"ICardConnectionServiceV1.Create:\nAttempt to create connection between already connected cards!\nCardLeft ({cardLeftId}), CardRight ({cardRightId})");
                throw new FunException("Эти карточки уже соединены!");
            }

            if (cardLeft.DeskId != cardRight.DeskId)
            {
                await TelegramAPI.Send($"ICardConnectionServiceV1.Create:\nAttempt to create connection between cards in different desks!\nCardLeft ({cardLeftId}), CardRight ({cardRightId})");
                throw new FunException("Невозможно соединить эти карточки, так как они находятся на разных досках.");
            }

            var desk = await _deskRepository.GetById(cardLeft.DeskId);

            // TODO: Support shared desks
            if (desk.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"ICardConnectionServiceV1.Create:\nAttempt to access restricted desk!\nDesk ({desk.Id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            var cardConnection = _mapper.Map<CardConnection>(createCardConnectionDto);

            await _cardConnectionRepository.Add(cardConnection);

            // TODO: Raise SSE event

            return cardConnection.Id;
        }

        async Task ICardConnectionServiceV1.Remove(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var cardConnection = await _cardConnectionRepository.GetById(id);

            var card = await _cardRepository.GetById(cardConnection.CardLeftId, c => c.Desk);

            // TODO: Support shared desks
            if (card.Desk.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"ICardConnectionServiceV1.Remove:\nAttempt to access restricted desk!\nDesk ({card.DeskId}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }

            await _cardConnectionRepository.Remove(cardConnection);

            // TODO: Raise SSE event
        }

        async Task<ICollection<CardConnectionWithIdDto>> ICardConnectionServiceV1.GetAllByDesk(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desk = await _deskRepository.GetById(id);

            // TODO: Support shared desks
            if (desk.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"ICardConnectionServiceV1.GetAllByDesk:\nAttempt to access restricted desk!\nDesk ({id}), Account({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске");
            }
            
            var cardConnections = await _cardConnectionRepository.GetMany(c => c.CardLeft.DeskId == id);

            var cardConnectionWithIdDtos = _mapper.Map<ICollection<CardConnectionWithIdDto>>(cardConnections);

            return cardConnectionWithIdDtos;
        }
    }
}