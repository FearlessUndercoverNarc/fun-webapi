using System;
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
        private IFolderShareRepository _folderShareRepository;
        private IFolderRepository _folderRepository;
        private IDeskRepository _deskRepository;
        private IMapper _mapper;

        public FolderService(IFolderRepository folderRepository, IMapper mapper, IRequestAccountIdService requestAccountIdService, IDeskRepository deskRepository, IFolderShareRepository folderShareRepository)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
            _requestAccountIdService = requestAccountIdService;
            _deskRepository = deskRepository;
            _folderShareRepository = folderShareRepository;
        }

        async Task<CreatedDto> IFolderServiceV1.Create(CreateFolderDto createFolderDto)
        {
            var requestAccountId = _requestAccountIdService.Id;

            if (createFolderDto.ParentId is { } parentId)
            {
                var parentFolder = await _folderRepository.GetById(parentId);
                // parentFolder can't be null, it's ID is checked in DTO

                if (!(parentFolder.AuthorAccountId == requestAccountId || await _folderShareRepository.HasSharedWriteTo(parentFolder.Id, requestAccountId)))
                {
                    await TelegramAPI.Send($"IFolderServiceV1.Create:\nAttempt to create folder in restricted location!\nFolderId ({parentId})\nUser ({requestAccountId})");
                    throw new FunException("Вы не можете создавать здесь что-либо, так как не являетесь владельцем");
                }
            }

            var folder = _mapper.Map<Folder>(createFolderDto);

            folder.AuthorAccountId = requestAccountId;
            folder.CreatedAt = DateTime.Now;
            folder.LastUpdatedAt = DateTime.Now;

            await _folderRepository.Add(folder);

            return folder.Id;
        }

        async Task IFolderServiceV1.Update(UpdateFolderDto updateFolderDto)
        {
            var folder = await _folderRepository.GetById(updateFolderDto.Id);

            long requestAccountId = _requestAccountIdService.Id;
            if (!(folder.AuthorAccountId == requestAccountId || await _folderShareRepository.HasSharedWriteTo(folder.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IFolderServiceV1.Update:\nAttempt to access restricted folder!\nFolderId ({updateFolderDto.Id})\nUser ({requestAccountId})");
                throw new FunException("Необходимо быть владельцем для внесения изменений!");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.Update:\nAttempt to access folder in trash bin!\nFolderId ({updateFolderDto.Id})\nUser ({requestAccountId})");
                throw new FunException("Нельзя изменять параметры элементов в корзине!\nВосстановите элемент для внесения изменений.");
            }

            _mapper.Map(updateFolderDto, folder);
            folder.LastUpdatedAt = DateTime.Now;

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

        async Task<ICollection<FolderWithIdDto>> IFolderServiceV1.GetSharedToMeRoots()
        {
            var requestAccountId = _requestAccountIdService.Id;

            // GetIndividuallyShared is already awared of trashbin
            var sharedRootIds = await _folderShareRepository.GetSharedRoots(requestAccountId);
            var folders = await _folderRepository.GetMany(
                f => sharedRootIds.Contains(f.Id),
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

            var requestAccountId = _requestAccountIdService.Id;

            if (!(parentFolder.AuthorAccountId == requestAccountId || await _folderShareRepository.HasSharedWriteTo(parentFolder.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IFolderServiceV1.GetSubfoldersByFolder:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({requestAccountId})");
                throw new FunException("У вас нет доступа к этой папке");
            }

            var folders = await _folderRepository.GetMany(
                f => f.ParentId == id && !f.IsInTrashBin,
                f => f.Desks.Where(d => !d.IsInTrashBin)
            );

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        async Task IFolderServiceV1.MoveToFolder(long id, long? destinationId)
        {
            var folder = await _folderRepository.GetById(id);

            var requestAccountId = _requestAccountIdService.Id;
            if (!(folder.AuthorAccountId == requestAccountId || await _folderShareRepository.HasSharedWriteTo(folder.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToFolder:\nAttempt to access restricted folder!\nFolderId ({id}) -> ({destinationId})\nUser ({requestAccountId})");
                throw new FunException("Вы не можете перемещать этот элемент");
            }

            if (folder.IsInTrashBin)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToFolder:\nAttempt to move folder in trash bin!\nFolderId ({id}) -> ({destinationId})\nUser ({requestAccountId})");
                throw new FunException("Перемещение элементов в корзине запрещено");
            }

            if (id == destinationId)
            {
                await TelegramAPI.Send($"IFolderServiceV1.MoveToFolder:\nAttempt to move folder into itself!\nFolderId ({id}) -> ({destinationId})\nUser ({requestAccountId})");
                throw new FunException("Вы пытаетесь переместить элемент внутрь себя");
            }

            // If we aren't moving into root, ensure that destination is available
            if (destinationId is not null)
            {
                var destinationFolder = await _folderRepository.GetById(destinationId.Value);

                if (!(destinationFolder.AuthorAccountId == requestAccountId || await _folderShareRepository.HasSharedWriteTo(destinationFolder.Id, requestAccountId)))
                {
                    await TelegramAPI.Send($"IFolderServiceV1.MoveToFolder:\nAttempt to move folder into restricted folder!\nFolderId ({id}) -> ({destinationId})\nUser ({requestAccountId})");
                    throw new FunException("Вы не можете перемещать в эту папку, так как не являетесь её владельцем");
                }
            }
            else if (folder.AuthorAccountId != requestAccountId)
            {
                // we need to ensure that destination folder is from same account if we move to root
                await TelegramAPI.Send($"IDeskServiceV1.MoveToFolder:\nAttempt to move shared element into folder of another account's root!\nDeskId ({folder.Id})\nUser ({requestAccountId})");
                throw new FunException("Вы не можете выполнить перемещение в корень дела другого пользователя");
            }

            folder.ParentId = destinationId;
            folder.LastUpdatedAt = DateTime.Now;

            await _folderRepository.Update(folder);
        }
    }
}