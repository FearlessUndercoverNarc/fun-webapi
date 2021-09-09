using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Models.DTOs.Misc;
using Models.Misc;
using Services.External;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class FolderService : IFolderServiceV2
    {
        async Task<CreatedDto> IFolderServiceV2.Create(CreateFolderDto createFolderDto)
        {
            if (createFolderDto.ParentId is { } parentId)
            {
                var parentFolder = await _folderRepository.GetById(parentId);
                // parentFolder can't be null, it's ID is checked in DTO

                if (parentFolder.AuthorAccountId != _requestAccountIdService.Id)
                {
                    await TelegramAPI.Send($"IFolderServiceV2.Create:\nAttempt to create folder in restricted location!\nFolderId ({parentId})\nUser ({_requestAccountIdService.Id})");
                    throw new FunException("Вы не можете создавать здесь что-либо, так как не являетесь владельцем");
                }
            }

            var folder = _mapper.Map<Folder>(createFolderDto);

            folder.AuthorAccountId = _requestAccountIdService.Id;
            folder.CreatedAt = DateTime.Now;
            folder.LastUpdatedAt = DateTime.Now;

            await _folderRepository.Add(folder);

            return folder.Id;
        }

        async Task IFolderServiceV2.Update(UpdateFolderDto updateFolderDto)
        {
            var folder = await _folderRepository.GetById(updateFolderDto.Id);

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV2.Update:\nAttempt to access restricted folder!\nFolderId ({updateFolderDto.Id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Необходимо быть владельцем для внесения изменений!");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV2.Update:\nAttempt to access folder in trash bin!\nFolderId ({updateFolderDto.Id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Нельзя изменять параметры элементов в корзине!\nВосстановите элемент для внесения изменений.");
            }

            _mapper.Map(updateFolderDto, folder);
            folder.LastUpdatedAt = DateTime.Now;

            await _folderRepository.Update(folder);
        }

        async Task<ICollection<FolderWithIdDto>> IFolderServiceV2.GetMyRoot()
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folders = await _folderRepository.GetMany(
                f => f.ParentId == null && f.AuthorAccountId == requestAccountId && !f.IsInTrashBin,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        async Task<ICollection<FolderWithIdDto>> IFolderServiceV2.GetSubfoldersByFolder(long id)
        {
            var parentFolder = await _folderRepository.GetById(
                id,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            if (parentFolder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV2.GetSubfoldersByFolder:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к этой папке");
            }

            // TODO: Support shared folders

            var folders = await _folderRepository.GetMany(
                f => f.ParentId == id && !f.IsInTrashBin,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        async Task IFolderServiceV2.MoveToTrashBin(long id)
        {
            var folder = await _folderRepository.GetById(id,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            // TODO: Support shared folders
            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV2.MoveToTrashBin:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Необходимо быть владельцем для удаления элемента!");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV2.MoveToTrashBin:\nAttempt to access folder in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Этот элемент уже в корзине");
            }

            
            folder.IsInTrashBin = true;
            foreach (var desk in folder.Desks)
            {
                desk.IsInTrashBin = true;
                desk.LastUpdatedAt = DateTime.Now;
            }

            folder.LastUpdatedAt = DateTime.Now;

            await _folderRepository.Update(folder);
        }

        async Task<ICollection<FolderWithIdDto>> IFolderServiceV2.GetMyTrashBin()
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folders = await _folderRepository.GetMany(
                f => f.Parent == null && f.AuthorAccountId == requestAccountId && f.IsInTrashBin,
                f => f.Desks
            );

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        async Task IFolderServiceV2.RestoreFromTrashBin(long id)
        {
            var folder = await _folderRepository.GetById(
                id,
                f => f.Desks.Where(d => d.IsInTrashBin)
            );

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV2.RestoreFromTrashBin:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к восстановлению этого элемента!");
            }

            if (!folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV2.RestoreFromTrashBin:\nAttempt to access folder not in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент не в корзине");
            }

            folder.IsInTrashBin = false;
            foreach (var desk in folder.Desks)
            {
                desk.IsInTrashBin = false;
                desk.LastUpdatedAt = DateTime.Now;
            }

            folder.LastUpdatedAt = DateTime.Now;

            await _folderRepository.Update(folder);
        }

        public async Task RemoveFromTrashBin(long id)
        {
            var folder = await _folderRepository.GetById(id,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV1.RemoveFromTrashBin:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к удалению этого элемента!");
            }

            if (!folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.RemoveFromTrashBin:\nAttempt to access folder not in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент не в корзине");
            }

            // HACK: we really need to delete only the first descendants, because the others aren't ever visible to user
            
            foreach (var desk in folder.Desks)
            {
                desk.LastUpdatedAt = DateTime.Now;
            }

            folder.LastUpdatedAt = DateTime.Now;

            await _deskRepository.RemoveMany(folder.Desks);
            await _folderRepository.RemoveMany(folder.Children);

            await _folderRepository.Remove(folder);
        }

        async Task IFolderServiceV2.MoveToFolder(long id, long? destinationId)
        {
            var folder = await _folderRepository.GetById(id);

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV2.MoveToFolder:\nAttempt to access restricted folder!\nFolderId ({id}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете перемещать этот элемент");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV2.MoveToFolder:\nAttempt to move folder in trash bin!\nFolderId ({id}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Перемещение элементов в корзине запрещено");
            }

            if (id == destinationId)
            {
                await TelegramAPI.Send($"IFolderServiceV2.MoveToFolder:\nAttempt to move folder into itself!\nFolderId ({id}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы пытаетесь переместить элемент внутрь себя");
            }

            // If we aren't moving into root, ensure that destination is available
            if (destinationId is not null)
            {
                var destinationFolder = await _folderRepository.GetById(destinationId.Value);

                // TODO: Support shared folders
                if (destinationFolder.AuthorAccountId != _requestAccountIdService.Id)
                {
                    await TelegramAPI.Send($"IFolderServiceV2.MoveToFolder:\nAttempt to move folder into restricted folder!\nFolderId ({id}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                    throw new FunException("Вы не можете перемещать в эту папку, так как не являетесь её владельцем");
                }
            }

            folder.ParentId = destinationId;
            folder.LastUpdatedAt = DateTime.Now;

            await _folderRepository.Update(folder);
        }
    }
}