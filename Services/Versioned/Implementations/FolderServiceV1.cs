using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Models.DTOs.Misc;
using Models.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class FolderService : IFolderServiceV1
    {
        private IRequestAccountIdService _requestAccountIdService;
        private IFolderRepository _folderRepository;
        private IMapper _mapper;

        public FolderService(IFolderRepository folderRepository, IMapper mapper, IRequestAccountIdService requestAccountIdService)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
            _requestAccountIdService = requestAccountIdService;
        }

        async Task<CreatedDto> IFolderServiceV1.Create(CreateFolderDto createFolderDto)
        {
            if (createFolderDto.ParentId is { } parentId)
            {
                var parentFolder = await _folderRepository.GetById(parentId);
                // parentFolder can't be null, it's ID is checked in DTO

                if (parentFolder.AuthorAccountId != _requestAccountIdService.Id)
                {
                    await TelegramAPI.Send($"IFolderServiceV1.Create:\nAttempt to create folder in restricted location!\nFolderId ({parentId})\nUser ({_requestAccountIdService.Id})");
                    throw new FunException("Вы не можете создавать здесь что-либо, так как не являетесь владельцем");
                }
            }

            var folder = _mapper.Map<Folder>(createFolderDto);

            folder.AuthorAccountId = _requestAccountIdService.Id;

            await _folderRepository.Add(folder);

            return folder.Id;
        }

        async Task IFolderServiceV1.Update(UpdateFolderDto updateFolderDto)
        {
            var folder = await _folderRepository.GetById(updateFolderDto.Id);

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV1.Update:\nAttempt to access restricted folder!\nFolderId ({updateFolderDto.Id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Необходимо быть владельцем для внесения изменений!");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.Update:\nAttempt to access folder in trash bin!\nFolderId ({updateFolderDto.Id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Нельзя изменять параметры элементов в корзине!\nВосстановите элемент для внесения изменений.");
            }

            _mapper.Map(updateFolderDto, folder);

            await _folderRepository.Update(folder);
        }

        async Task<ICollection<FolderWithIdDto>> IFolderServiceV1.GetMyRoot()
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folders = await _folderRepository.GetMany(
                f => f.ParentId == null && f.AuthorAccountId == requestAccountId && !f.IsInTrashBin,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        async Task<ICollection<FolderWithIdDto>> IFolderServiceV1.GetSubfoldersByFolder(long id)
        {
            var parentFolder = await _folderRepository.GetById(
                id,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            if (parentFolder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV1.GetSubfoldersByFolder:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
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

        async Task IFolderServiceV1.MoveToTrashBin(long id)
        {
            var folder = await _folderRepository.GetById(id,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            // TODO: Support shared folders
            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToTrashBin:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Необходимо быть владельцем для удаления элемента!");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToTrashBin:\nAttempt to access folder in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Этот элемент уже в корзине");
            }

            folder.IsInTrashBin = true;
            foreach (var desk in folder.Desks)
            {
                desk.IsInTrashBin = true;
            }

            await _folderRepository.Update(folder);
        }

        async Task<ICollection<FolderWithIdDto>> IFolderServiceV1.GetMyTrashBin()
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folders = await _folderRepository.GetMany(
                f => f.AuthorAccountId == requestAccountId && f.IsInTrashBin,
                f => f.Desks
            );

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        async Task IFolderServiceV1.RestoreFromTrashBin(long folderId)
        {
            var folder = await _folderRepository.GetById(folderId,
                f => f.Desks.Where(d => d.IsInTrashBin)
            );

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV1.RestoreFromTrashBin:\nAttempt to access restricted folder!\nFolderId ({folderId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("У вас нет доступа к восстановлению этого элемента!");
            }

            if (!folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.RestoreFromTrashBin:\nAttempt to access folder not in trash bin!\nFolderId ({folderId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент не в корзине");
            }

            folder.IsInTrashBin = false;
            foreach (var desk in folder.Desks)
            {
                desk.IsInTrashBin = false;
            }

            await _folderRepository.Update(folder);
        }

        async Task IFolderServiceV1.MoveToFolder(long folderId, long? destinationId)
        {
            var folder = await _folderRepository.GetById(folderId);

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToFolder:\nAttempt to access restricted folder!\nFolderId ({folderId}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете перемещать этот элемент");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToFolder:\nAttempt to move folder in trash bin!\nFolderId ({folderId}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Перемещение элементов в корзине запрещено");
            }

            if (folderId == destinationId)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToFolder:\nAttempt to move folder into itself!\nFolderId ({folderId}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы пытаетесь переместить элемент внутрь себя");
            }

            // If we aren't moving into root, ensure that destination is available
            if (destinationId is not null)
            {
                var destinationFolder = await _folderRepository.GetById(destinationId.Value);

                // TODO: Support shared folders
                if (destinationFolder.AuthorAccountId != _requestAccountIdService.Id)
                {
                    await TelegramAPI.Send($"IFolderServiceV1.MoveToFolder:\nAttempt to move folder into restricted folder!\nFolderId ({folderId}) -> ({destinationId})\nUser ({_requestAccountIdService.Id})");
                    throw new FunException("Вы не можете перемещать в эту папку, так как не являетесь её владельцем");
                }
            }

            folder.ParentId = destinationId;

            await _folderRepository.Update(folder);
        }
    }
}