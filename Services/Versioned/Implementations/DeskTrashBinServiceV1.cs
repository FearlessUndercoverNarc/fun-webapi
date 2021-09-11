using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.DTOs.Desks;
using Models.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class DeskTrashBinService : IDeskTrashBinServiceV1
    {
        private IDeskRepository _deskRepository;
        private IFolderRepository _folderRepository;
        private IMapper _mapper;
        private IRequestAccountIdService _requestAccountIdService;
        private IDeskShareRepository _deskShareRepository;
        private IFolderShareRepository _folderShareRepository;

        public DeskTrashBinService(IDeskRepository deskRepository, IFolderRepository folderRepository, IMapper mapper, IRequestAccountIdService requestAccountIdService, IDeskShareRepository deskShareRepository, IFolderShareRepository folderShareRepository)
        {
            _deskRepository = deskRepository;
            _folderRepository = folderRepository;
            _mapper = mapper;
            _requestAccountIdService = requestAccountIdService;
            _deskShareRepository = deskShareRepository;
            _folderShareRepository = folderShareRepository;
        }

        async Task<ICollection<DeskWithIdDto>> IDeskTrashBinServiceV1.GetMyTrashBin()
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desks = await _deskRepository.GetMany(
                d => d.AuthorAccountId == requestAccountId && d.IsInTrashBin && !d.Parent.IsInTrashBin,
                d => d.Parent,
                d => d.AuthorAccount
            );

            var deskWithIdDtos = _mapper.Map<ICollection<DeskWithIdDto>>(desks);

            return deskWithIdDtos;
        }

        async Task IDeskTrashBinServiceV1.MoveToTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToTrashBin:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете переместить эту доску в корзину, т.к. не являетесь её владельцем!");
            }

            if (desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToTrashBin:\nAttempt to access desk in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент уже в корзине");
            }

            desk.IsInTrashBin = true;
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }

        async Task IDeskTrashBinServiceV1.RestoreFromTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.RestoreFromTrashBin:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете восстановить данную доску из корзины, т.к. не являетесь её владельцем!");
            }

            if (!desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV1.RestoreFromTrashBin:\nAttempt to access desk not in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент не в корзине");
            }

            desk.IsInTrashBin = false;
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }

        async Task IDeskTrashBinServiceV1.RemoveFromTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.RemoveFromTrashBin:\nAttempt to access restricted folder!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете удалять этот элемент");
            }

            if (!desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV1.RemoveFromTrashBin:\nAttempt to remove element not in trash bin!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Удаление элементов вне корзины запрещено");
            }

            desk.LastUpdatedAt = DateTime.Now;
            await _deskRepository.Remove(desk);
        }
    }
}