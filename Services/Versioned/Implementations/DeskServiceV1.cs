using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.Desks;
using Models.DTOs.Misc;
using Models.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class DeskService : IDeskServiceV1
    {
        private IDeskRepository _deskRepository;
        private IFolderRepository _folderRepository;
        private IMapper _mapper;
        private IRequestAccountIdService _requestAccountIdService;
        private IDeskShareRepository _deskShareRepository;

        public DeskService(IDeskRepository deskRepository, IMapper mapper, IRequestAccountIdService requestAccountIdService, IFolderRepository folderRepository, IDeskShareRepository deskShareRepository)
        {
            _deskRepository = deskRepository;
            _mapper = mapper;
            _requestAccountIdService = requestAccountIdService;
            _folderRepository = folderRepository;
            _deskShareRepository = deskShareRepository;
        }

        async Task<CreatedDto> IDeskServiceV1.Create(CreateDeskDto createDeskDto)
        {
            if (createDeskDto.ParentId is { } parentId)
            {
                var parentFolder = await _folderRepository.GetById(parentId);
                // parentFolder can't be null, it's ID is checked in DTO

                if (parentFolder.AuthorAccountId != _requestAccountIdService.Id)
                {
                    await TelegramAPI.Send($"IDeskServiceV1.Create:\nAttempt to create desk in restricted location!\nFolderId ({parentId})\nUser ({_requestAccountIdService.Id})");
                    throw new FunException("Вы не можете создавать здесь что-либо, так как не являетесь владельцем");
                }
            }

            var desk = _mapper.Map<Desk>(createDeskDto);

            desk.CreatedAt = DateTime.Now;
            desk.LastUpdatedAt = DateTime.Now;
            desk.AuthorAccountId = _requestAccountIdService.Id;

            await _deskRepository.Add(desk);

            return desk.Id;
        }

        async Task IDeskServiceV1.Update(UpdateDeskDto updateDeskDto)
        {
            var desk = await _deskRepository.GetById(updateDeskDto.Id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.Update:\nAttempt to access restricted desk!\nFolderId ({updateDeskDto.Id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Необходимо быть владельцем для внесения изменений!");
            }

            if (desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV1.Update:\nAttempt to access desk in trash bin!\nFolderId ({updateDeskDto.Id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Нельзя изменять параметры элементов в корзине!\nВосстановите элемент для внесения изменений.");
            }

            _mapper.Map(updateDeskDto, desk);
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }

        async Task<DeskWithIdDto> IDeskServiceV1.GetById(long id)
        {
            var desk = await _deskRepository.GetById(
                id,
                d => d.Parent,
                d => d.AuthorAccount
            );

            // TODO: Support shared folders
            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.GetById:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к этой доске!");
            }

            var deskWithIdDto = _mapper.Map<DeskWithIdDto>(desk);

            return deskWithIdDto;
        }

        async Task<ICollection<DeskWithIdDto>> IDeskServiceV1.GetByFolder(long folderId)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var folder = await _folderRepository.GetById(folderId);

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.GetByFolder:\nAttempt to access desks in restricted location!\nFolderId ({folderId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете создавать здесь что-либо, так как не являетесь владельцем");
            }

            var desks = await _deskRepository.GetMany(
                d => d.ParentId == folderId && d.AuthorAccountId == requestAccountId && !d.IsInTrashBin,
                d => d.Parent,
                d => d.AuthorAccount
            );

            var deskWithIdDtos = _mapper.Map<ICollection<DeskWithIdDto>>(desks);

            return deskWithIdDtos;
        }

        async Task<ICollection<DeskWithIdDto>> IDeskServiceV1.GetSharedToMe()
        {
            var requestAccountId = _requestAccountIdService.Id;

            // GetIndividuallyShared is already aware of trashbin
            var individuallySharedDeskIds = await _deskShareRepository.GetIndividuallyShared(requestAccountId);

            var desks = await _deskRepository.GetMany(
                d => individuallySharedDeskIds.Contains(d.Id),
                d => d.Parent,
                d => d.AuthorAccount
            );

            var deskWithIdDtos = _mapper.Map<ICollection<DeskWithIdDto>>(desks);

            return deskWithIdDtos;
        }

        async Task<ICollection<DeskWithIdDto>> IDeskServiceV1.GetMyTrashBin()
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desks = await _deskRepository.GetMany(
                d => d.AuthorAccountId == requestAccountId && d.IsInTrashBin,
                d => d.Parent,
                d => d.AuthorAccount
            );

            var deskWithIdDtos = _mapper.Map<ICollection<DeskWithIdDto>>(desks);

            return deskWithIdDtos;
        }

        async Task IDeskServiceV1.MoveToTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToTrashBin:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к этой доске!");
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

        async Task IDeskServiceV1.RestoreFromTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.RestoreFromTrashBin:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к этой доске!");
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

        async Task IDeskServiceV1.RemoveFromTrashBin(long id)
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

        async Task IDeskServiceV1.MoveToFolder(long deskId, long destinationId)
        {
            var desk = await _deskRepository.GetById(deskId);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to access restricted folder!\nDeskId ({deskId}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете перемещать этот элемент");
            }

            if (desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to move element in trash bin!\nDeskId ({deskId}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Перемещение элементов в корзине запрещено");
            }

            if (desk.ParentId == destinationId)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to move element into same location!\nDeskId ({deskId}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы пытаетесь переместить элемент внутрь себя");
            }

            var destinationFolder = await _folderRepository.GetById(destinationId);

            // TODO: Support shared folders
            if (destinationFolder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to move element into restricted folder!\nDeskId ({deskId}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете перемещать в эту элемент, так как не являетесь её владельцем");
            }

            desk.ParentId = destinationId;
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }
    }
}