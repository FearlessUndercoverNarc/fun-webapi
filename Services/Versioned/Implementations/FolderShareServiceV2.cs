using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.Db.Relations;
using Models.Misc;
using Services.External;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class FolderShareService : IFolderShareServiceV2
    {
        async Task IFolderShareServiceV2.Share(long id, long recipientId)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var shareRoot = await _folderRepository.GetById(id);

            if (shareRoot.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IFolderShareServiceV2.Share:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете управлять доступом к этой папке, так как не являетесь её владельцем.");
            }

            List<long> folders = new();
            await AggregateUnsharedFolders(id, recipientId, folders);

            List<FolderShare> shares = new List<FolderShare>(folders.Count);

            foreach (var folder in folders)
            {
                shares.Add(new FolderShare() {FolderId = folder, FunAccountId = recipientId});
            }

            await _folderShareRepository.AddMany(shares);
        }

        async Task IFolderShareServiceV2.RemoveShare(long id, long recipientId)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var shareRoot = await _folderRepository.GetById(id);

            if (shareRoot.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IFolderShareServiceV2.RemoveShare:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете управлять доступом к этой папке, так как не являетесь её владельцем.");
            }

            // if this folder is a subfolder, we must ensure that it's parent is not shared
            // because it will cause a partial exclusion, which is EXTREMELY hard to maintain
            if (await _folderRepository.IsParentSharedTo(id, recipientId))
            {
                await TelegramAPI.Send($"IFolderShareServiceV1.RemoveShare:\nAttempt to remove share in another share!\nFolder ({id}), FunAccount({recipientId})");
                throw new FunException("Вы не можете убрать доступ к этому ресурсу, так как он находится внутри другого публичного ресурса!");
            }

            List<long> folders = new();
            await AggregateSharedFolders(id, recipientId, folders);

            var errorsStringBuilder = new StringBuilder();
            List<FolderShare> shares = new();

            foreach (var folder in folders)
            {
                var folderShare = await _folderShareRepository.GetOne(s => s.FolderId == folder && s.FunAccountId == recipientId);

                if (folderShare == null)
                {
                    errorsStringBuilder.AppendLine($"FolderShare not found for subfolder ({folder}) and Account ({recipientId})");
                    continue;
                }

                shares.Add(folderShare);
            }

            if (errorsStringBuilder.Length != 0)
            {
                await TelegramAPI.Send($"IFolderShareServiceV2.RemoveShare:\nAn inner error has occured while removing share!\n{errorsStringBuilder}");
            }

            await _folderShareRepository.RemoveMany(shares);
        }
    }
}