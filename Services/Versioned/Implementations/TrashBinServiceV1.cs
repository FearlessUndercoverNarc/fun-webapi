using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Models.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class FolderTrashBinService : IFolderTrashBinServiceV1
    {
        private IRequestAccountIdService _requestAccountIdService;
        private IFolderShareRepository _folderShareRepository;
        private IFolderRepository _folderRepository;
        private IDeskRepository _deskRepository;
        private IMapper _mapper;

        public FolderTrashBinService(IRequestAccountIdService requestAccountIdService, IFolderShareRepository folderShareRepository, IFolderRepository folderRepository, IDeskRepository deskRepository, IMapper mapper)
        {
            _requestAccountIdService = requestAccountIdService;
            _folderShareRepository = folderShareRepository;
            _folderRepository = folderRepository;
            _deskRepository = deskRepository;
            _mapper = mapper;
        }
        
        private async Task AggregateNonTrashed(long id, List<long> folders, List<long> desks)
        {
            var folder = await _folderRepository.GetById(
                id,
                f => f.Children,
                f => f.Desks
            );

            if (!folder.IsInTrashBin)
            {
                folders.Add(id);
            }

            foreach (var child in folder.Children)
            {
                await AggregateNonTrashed(child.Id, folders, desks);
            }

            desks.AddRange(from desk in folder.Desks where !desk.IsInTrashBin select desk.Id);
        }
        
        private async Task AggregateTrashed(long id, List<long> folders, List<long> desks)
        {
            var folder = await _folderRepository.GetById(
                id,
                f => f.Children,
                f => f.Desks
            );

            if (folder.IsInTrashBin)
            {
                folders.Add(id);
            }

            foreach (var child in folder.Children)
            {
                await AggregateNonTrashed(child.Id, folders, desks);
            }

            desks.AddRange(from desk in folder.Desks where desk.IsInTrashBin select desk.Id);
        }

        async Task<ICollection<FolderWithIdDto>> IFolderTrashBinServiceV1.GetMyTrashBin()
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folders = await _folderRepository.GetMany(
                f => f.AuthorAccountId == requestAccountId && f.IsInTrashBin && !f.Parent.IsInTrashBin,
                f => f.Desks
            );

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        async Task IFolderTrashBinServiceV1.MoveToTrashBin(long id)
        {
            var folder = await _folderRepository.GetById(
                id,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            var requestAccountId = _requestAccountIdService.Id;
            if (folder.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToTrashBin:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({requestAccountId})");
                throw new FunException("Необходимо быть владельцем для удаления элемента!");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToTrashBin:\nAttempt to access folder in trash bin!\nFolderId ({id})\nUser ({requestAccountId})");
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

        async Task IFolderTrashBinServiceV1.RestoreFromTrashBin(long id)
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

        async Task IFolderTrashBinServiceV1.RemoveFromTrashBin(long id)
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