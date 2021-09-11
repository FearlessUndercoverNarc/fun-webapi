using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Models.Misc;
using Services.External;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class FolderTrashBinService : IFolderTrashBinServiceV2
    {
        async Task<ICollection<FolderWithIdDto>> IFolderTrashBinServiceV2.GetMyTrashBin()
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folders = await _folderRepository.GetMany(
                f => f.AuthorAccountId == requestAccountId && f.IsInTrashBin && !f.Parent.IsInTrashBin,
                f => f.Desks
            );

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        async Task IFolderTrashBinServiceV2.MoveToTrashBin(long id)
        {
            var folder = await _folderRepository.GetById(
                id,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            var requestAccountId = _requestAccountIdService.Id;
            if (folder.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IFolderServiceV2.MoveToTrashBin:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({requestAccountId})");
                throw new FunException("Необходимо быть владельцем для удаления элемента!");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV2.MoveToTrashBin:\nAttempt to access folder in trash bin!\nFolderId ({id})\nUser ({requestAccountId})");
                throw new FunException("Этот элемент уже в корзине");
            }

            List<long> folders = new();
            List<long> desks = new();
            await AggregateNonTrashed(folder.Id, folders, desks);

            List<Folder> foldersToTrash = new(folders.Count);
            List<Desk> desksToTrash = new(desks.Count);

            foreach (var f in folders)
            {
                var folderToTrash = await _folderRepository.GetById(f);

                folderToTrash.IsInTrashBin = true;
                foldersToTrash.Add(folderToTrash);
            }

            foreach (var d in desks)
            {
                var deskToTrash = await _deskRepository.GetById(d);

                deskToTrash.IsInTrashBin = true;
                desksToTrash.Add(deskToTrash);
            }

            await _folderRepository.UpdateMany(foldersToTrash);
            await _deskRepository.UpdateMany(desksToTrash);
        }

        async Task IFolderTrashBinServiceV2.RestoreFromTrashBin(long id)
        {
            var folder = await _folderRepository.GetById(id,
                f => f.Parent,
                f => f.Desks.Where(d => d.IsInTrashBin)
            );

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV1.RestoreFromTrashBin:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к восстановлению этого элемента!");
            }

            if (!folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.RestoreFromTrashBin:\nAttempt to access folder not in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент не в корзине");
            }

            if (folder.Parent is {IsInTrashBin: true})
            {
                await TelegramAPI.Send($"IFolderServiceV1.RestoreFromTrashBin:\nAttempt to restore folder in trashed folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете восстановить этот элемент, т.к. он находится внутри удалённого элемента. Сначала восстановите родительский элемент.");
            }

            List<long> folders = new();
            List<long> desks = new();
            await AggregateTrashed(folder.Id, folders, desks);

            List<Folder> foldersToTrash = new(folders.Count);
            List<Desk> desksToTrash = new(desks.Count);

            foreach (var f in folders)
            {
                var folderToTrash = await _folderRepository.GetById(f);

                folderToTrash.IsInTrashBin = false;
                foldersToTrash.Add(folderToTrash);
            }

            foreach (var d in desks)
            {
                var deskToTrash = await _deskRepository.GetById(d);

                deskToTrash.IsInTrashBin = false;
                desksToTrash.Add(deskToTrash);
            }

            await _folderRepository.UpdateMany(foldersToTrash);
            await _deskRepository.UpdateMany(desksToTrash);
        }

        async Task IFolderTrashBinServiceV2.RemoveFromTrashBin(long id)
        {
            var folder = await _folderRepository.GetById(id,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV2.RemoveFromTrashBin:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к удалению этого элемента!");
            }

            if (!folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV2.RemoveFromTrashBin:\nAttempt to access folder not in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент не в корзине");
            }

            List<long> folders = new();
            List<long> desks = new();
            await AggregateTrashed(folder.Id, folders, desks);

            var foldersToRemove = await _folderRepository.GetMany(f => folders.Contains(f.Id));
            var desksToRemove = await _deskRepository.GetMany(d => desks.Contains(d.Id));

            await _folderRepository.RemoveMany(foldersToRemove);
            await _deskRepository.RemoveMany(desksToRemove);
        }
    }
}