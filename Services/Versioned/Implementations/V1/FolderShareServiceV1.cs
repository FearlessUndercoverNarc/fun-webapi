using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Relations;
using Models.DTOs.Relations;
using Models.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class FolderShareService : IFolderShareServiceV1
    {
        private IFolderRepository _folderRepository;
        private IFolderShareRepository _folderShareRepository;
        private IDeskRepository _deskRepository;
        private IDeskShareRepository _deskShareRepository;
        private IRequestAccountIdService _requestAccountIdService;
        private IMapper _mapper;

        public FolderShareService(IFolderRepository folderRepository, IFolderShareRepository folderShareRepository, IRequestAccountIdService requestAccountIdService, IDeskShareRepository deskShareRepository, IDeskRepository deskRepository, IMapper mapper)
        {
            _folderRepository = folderRepository;
            _folderShareRepository = folderShareRepository;
            _requestAccountIdService = requestAccountIdService;
            _deskShareRepository = deskShareRepository;
            _deskRepository = deskRepository;
            _mapper = mapper;
        }

        private async Task AggregateUnshared(long id, long recipientId, List<long> folders, List<long> desks)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folder = await _folderRepository.GetById(
                id,
                f => f.Children,
                f => f.Desks
            );

            if (!await _folderShareRepository.HasSharedReadTo(id, recipientId))
            {
                // this folder is not shared to this user
                folders.Add(id);
            }

            foreach (var child in folder.Children)
            {
                await AggregateUnshared(child.Id, recipientId, folders, desks);
            }

            foreach (var desk in folder.Desks)
            {
                if (!await _deskShareRepository.HasSharedReadTo(desk.Id, recipientId))
                {
                    desks.Add(desk.Id);
                }
            }
        }

        private async Task AggregateShared(long id, long recipientId, List<long> folders, List<long> desks)
        {
            var folder = await _folderRepository.GetById(
                id,
                f => f.Children,
                f => f.Desks
            );

            if (await _folderShareRepository.HasSharedReadTo(id, recipientId))
            {
                // this folder is shared to this user
                folders.Add(id);
            }

            foreach (var child in folder.Children)
            {
                await AggregateShared(child.Id, recipientId, folders, desks);
            }

            foreach (var desk in folder.Desks)
            {
                if (!await _deskShareRepository.HasSharedReadTo(desk.Id, recipientId))
                {
                    desks.Add(desk.Id);
                }
            }
        }

        async Task IFolderShareServiceV1.Share(long id, long recipientId, bool hasWriteAccess)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var shareRoot = await _folderRepository.GetById(id);

            if (shareRoot.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IFolderShareServiceV1.Share:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете управлять доступом к этой папке, так как не являетесь её владельцем.");
            }

            List<long> folders = new();
            List<long> desks = new();
            await AggregateUnshared(id, recipientId, folders, desks);

            List<FolderShare> folderShares = new List<FolderShare>(folders.Count);
            List<DeskShare> deskShares = new List<DeskShare>(desks.Count);

            foreach (var folder in folders)
            {
                folderShares.Add(new FolderShare()
                {
                    FolderId = folder,
                    FunAccountId = recipientId,
                    HasWriteAccess = hasWriteAccess
                });
            }

            foreach (var desk in desks)
            {
                deskShares.Add(new DeskShare()
                {
                    DeskId = desk,
                    FunAccountId = recipientId,
                    HasWriteAccess = hasWriteAccess
                });
            }

            await _folderShareRepository.AddMany(folderShares);
            await _deskShareRepository.AddMany(deskShares);
        }

        async Task<ICollection<FolderShareDto>> IFolderShareServiceV1.GetShares(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IFolderShareServiceV1.GetShares:\nAttempt to access restricted folder!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете просматривать информацию о доступе к этому ресурсу, так как не являетесь его владельцем.");
            }

            var folderShares = await _folderShareRepository.GetShares(id);

            var folderShareDtos = _mapper.Map<ICollection<FolderShareDto>>(folderShares);
            return folderShareDtos;
        }

        async Task IFolderShareServiceV1.RemoveShare(long id, long recipientId)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var shareRoot = await _folderRepository.GetById(id);

            if (shareRoot.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IFolderShareServiceV1.RemoveShare:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
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
            List<long> desks = new();
            await AggregateShared(id, recipientId, folders, desks);

            var folderSharingErrorsStringBuilder = new StringBuilder();
            List<FolderShare> folderShares = new();

            foreach (var folder in folders)
            {
                var folderShare = await _folderShareRepository.GetOne(s => s.FolderId == folder && s.FunAccountId == recipientId);

                if (folderShare == null)
                {
                    folderSharingErrorsStringBuilder.AppendLine($"FolderShare not found for subfolder ({folder}) and Account ({recipientId})");
                    continue;
                }

                folderShares.Add(folderShare);
            }

            if (folderSharingErrorsStringBuilder.Length != 0)
            {
                await TelegramAPI.Send($"IFolderShareServiceV1.RemoveShare:\nAn inner error has occured while removing share!\n{folderSharingErrorsStringBuilder}");
                throw new FunException("Произошла ошибка при удалении доступа к папке! Обработка была прервана. Разработчики уже уведомлены.");
            }

            var deskSharingErrorsStringBuilder = new StringBuilder();
            List<DeskShare> deskShares = new();

            foreach (var desk in desks)
            {
                var deskShare = await _deskShareRepository.GetOne(s => s.DeskId == desk && s.FunAccountId == recipientId);

                if (deskShare == null)
                {
                    deskSharingErrorsStringBuilder.AppendLine($"DeskShare not found for desk ({desk}) and Account ({recipientId})");
                    continue;
                }

                deskShares.Add(deskShare);
            }

            if (folderSharingErrorsStringBuilder.Length != 0)
            {
                await TelegramAPI.Send($"IFolderShareServiceV1.RemoveShare:\nAn inner error has occured while removing share!\n{folderSharingErrorsStringBuilder}");
                throw new FunException("Произошла ошибка при удалении доступа к доске! Обработка была прервана. Разработчики уже уведомлены.");
            }

            await _folderShareRepository.RemoveMany(folderShares);
            await _deskShareRepository.RemoveMany(deskShares);
        }
    }
}