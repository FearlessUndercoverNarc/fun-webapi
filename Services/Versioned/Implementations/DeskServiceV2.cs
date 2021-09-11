using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Db.Tree;
using Models.DTOs.Desks;
using Models.DTOs.Misc;
using Models.Misc;
using Services.External;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class DeskService : IDeskServiceV2
    {
        async Task<CreatedDto> IDeskServiceV2.Create(CreateDeskDto createDeskDto)
        {
            var requestAccountId = _requestAccountIdService.Id;
            if (createDeskDto.ParentId is { } parentId)
            {
                var parentFolder = await _folderRepository.GetById(parentId);
                // parentFolder can't be null, it's ID is checked in DTO

                // TODO: Support separate read/write accesses
                if (!(parentFolder.AuthorAccountId == requestAccountId || await _folderShareRepository.IsSharedTo(parentFolder.Id, requestAccountId)))
                {
                    await TelegramAPI.Send($"IDeskServiceV1.Create:\nAttempt to create desk in restricted location!\nFolderId ({parentId})\nUser ({requestAccountId})");
                    throw new FunException("Вы не можете создавать здесь что-либо, так как не являетесь владельцем");
                }
            }

            var desk = _mapper.Map<Desk>(createDeskDto);

            desk.CreatedAt = DateTime.Now;
            desk.LastUpdatedAt = DateTime.Now;
            desk.AuthorAccountId = requestAccountId;

            await _deskRepository.Add(desk);

            var deskActionHistoryItem = new DeskActionHistoryItem()
            {
                DeskId = desk.Id,
                DateTime = DateTime.Now,
                FunAccountId = requestAccountId,
                Version = 1,
                Action = ActionType.DeskInit,
                OldData = "",
                NewData = ""
            };

            await _deskActionHistoryRepository.Add(deskActionHistoryItem);

            return desk.Id;
        }

        async Task IDeskServiceV2.Update(UpdateDeskDto updateDeskDto)
        {
            var desk = await _deskRepository.GetById(updateDeskDto.Id);

            var requestAccountId = _requestAccountIdService.Id;

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IDeskServiceV1.Update:\nAttempt to access restricted desk!\nFolderId ({updateDeskDto.Id})\nUser ({requestAccountId})");
                throw new FunException("Необходимо быть владельцем для внесения изменений!");
            }

            if (desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV1.Update:\nAttempt to access desk in trash bin!\nFolderId ({updateDeskDto.Id})\nUser ({requestAccountId})");
                throw new FunException("Нельзя изменять параметры элементов в корзине!\nВосстановите элемент для внесения изменений.");
            }

            _mapper.Map(updateDeskDto, desk);
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);

            var deskActionHistoryItem = new DeskActionHistoryItem()
            {
                DeskId = desk.Id,
                DateTime = DateTime.Now,
                FunAccountId = requestAccountId,
                Version = 1,
                Action = ActionType.DeskUpdate,
                OldData = "",
                NewData = ""
            };

            await _deskActionHistoryRepository.Add(deskActionHistoryItem);
        }

        async Task<DeskWithIdDto> IDeskServiceV2.GetById(long id)
        {
            var desk = await _deskRepository.GetById(
                id,
                d => d.Parent,
                d => d.AuthorAccount
            );

            var requestAccountId = _requestAccountIdService.Id;
            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IDeskServiceV1.GetById:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({requestAccountId})");
                throw new FunException("У вас нет доступа к этой доске!");
            }

            var deskWithIdDto = _mapper.Map<DeskWithIdDto>(desk);

            return deskWithIdDto;
        }

        async Task<ICollection<DeskWithIdDto>> IDeskServiceV2.GetByFolder(long folderId)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var folder = await _folderRepository.GetById(folderId);

            if (!(folder.AuthorAccountId == requestAccountId || await _folderShareRepository.IsSharedTo(folder.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IDeskServiceV1.GetByFolder:\nAttempt to access desks in restricted location!\nFolderId ({folderId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете просматривать содержимое этой папки или дела, так как не являетесь владельцем");
            }

            var desks = await _deskRepository.GetMany(
                d => d.ParentId == folderId && d.AuthorAccountId == requestAccountId && !d.IsInTrashBin,
                d => d.Parent,
                d => d.AuthorAccount
            );

            var deskWithIdDtos = _mapper.Map<ICollection<DeskWithIdDto>>(desks);

            return deskWithIdDtos;
        }

        async Task<ICollection<DeskWithIdDto>> IDeskServiceV2.GetSharedToMe()
        {
            var requestAccountId = _requestAccountIdService.Id;

            // GetIndividuallyShared is already aware of trashbin
            var individuallySharedDeskIds = await _deskShareRepository.GetIndividuallyShared(requestAccountId);

            var desks = await _deskRepository.GetMany(d => individuallySharedDeskIds.Contains(d.Id));

            var deskWithIdDtos = _mapper.Map<ICollection<DeskWithIdDto>>(desks);

            return deskWithIdDtos;
        }

        async Task<ICollection<DeskWithIdDto>> IDeskServiceV2.GetMyTrashBin()
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

        async Task IDeskServiceV2.MoveToTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV2.MoveToTrashBin:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к этой доске!");
            }

            if (desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV2.MoveToTrashBin:\nAttempt to access desk in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент уже в корзине");
            }

            desk.IsInTrashBin = true;
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }

        async Task IDeskServiceV2.RestoreFromTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV2.RestoreFromTrashBin:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к этой доске!");
            }

            if (!desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV2.RestoreFromTrashBin:\nAttempt to access desk not in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент не в корзине");
            }

            desk.IsInTrashBin = false;
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }

        async Task IDeskServiceV2.RemoveFromTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV2.RemoveFromTrashBin:\nAttempt to access restricted folder!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете удалять этот элемент");
            }

            if (!desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV2.RemoveFromTrashBin:\nAttempt to remove element not in trash bin!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Удаление элементов вне корзины запрещено");
            }

            desk.LastUpdatedAt = DateTime.Now;
            await _deskRepository.Remove(desk);
        }

        async Task IDeskServiceV2.MoveToFolder(long deskId, long destinationId)
        {
            var desk = await _deskRepository.GetById(deskId);

            var requestAccountId = _requestAccountIdService.Id;

            if (!(desk.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(desk.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to access restricted folder!\nDeskId ({deskId}) -> ({destinationId})\nUser ({requestAccountId})");
                throw new FunException("Вы не можете перемещать этот элемент");
            }

            if (desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to move element in trash bin!\nDeskId ({deskId}) -> ({destinationId})\nUser ({requestAccountId})");
                throw new FunException("Перемещение элементов в корзине запрещено");
            }

            if (desk.ParentId == destinationId)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to move element into same location!\nDeskId ({deskId}) -> ({destinationId})\nUser ({requestAccountId})");
                throw new FunException("Вы пытаетесь переместить элемент внутрь себя");
            }

            // we need to check that destination can be edited by us (either by authoring, or being shared)
            var destinationFolder = await _folderRepository.GetById(destinationId);
            if (!(destinationFolder.AuthorAccountId == requestAccountId || await _deskShareRepository.IsSharedTo(destinationId, requestAccountId)))
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to move element into restricted folder!\nDeskId ({deskId}) -> ({destinationId})\nUser ({requestAccountId})");
                throw new FunException("Вы не можете перемещать в эту папку, так как не являетесь её владельцем");
            }
            
            // we need to ensure that source and destination folders are from same account
            var sourceFolder = await _folderRepository.GetById(desk.ParentId);
            if (sourceFolder.AuthorAccountId != destinationFolder.AuthorAccountId)
            {
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to move element into folder of another account!\nDeskId ({deskId}) -> ({destinationId})\nUser ({requestAccountId})");
                throw new FunException("Вы не можете выполнить данное перемещение, т.к. оно производится между папками/делами разных аккаунтами");
            }

            desk.ParentId = destinationId;
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }
    }
}