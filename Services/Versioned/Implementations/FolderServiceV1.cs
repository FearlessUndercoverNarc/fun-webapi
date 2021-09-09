using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Models.DTOs.Misc;
using Models.Misc;
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

        public async Task<CreatedDto> CreateV1(CreateFolderDto createFolderDto)
        {
            if (createFolderDto.ParentId is { } parentId)
            {
                var parentFolder = await _folderRepository.GetById(parentId);
                // parentFolder can't be null, it's checked in DTO

                if (parentFolder.AuthorAccountId != _requestAccountIdService.Id)
                {
                    throw new FunException("Вы не можете создавать здесь что-либо, так как не являетесь владельцем");
                }
            }

            var folder = _mapper.Map<Folder>(createFolderDto);

            folder.AuthorAccountId = _requestAccountIdService.Id;

            await _folderRepository.Add(folder);

            return folder.Id;
        }

        public async Task UpdateV1(UpdateFolderDto updateFolderDto)
        {
            var folder = await _folderRepository.GetById(updateFolderDto.Id);

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                throw new FunException("Необходимо быть владельцем для внесения изменений!");
            }

            // If we change the parent, we need to check it's availability
            if (folder.ParentId != updateFolderDto.ParentId)
            {
                if (updateFolderDto.ParentId is { } parentId)
                {
                    var parentFolder = await _folderRepository.GetById(parentId);
                    // parentFolder can't be null, it's checked in DTO

                    if (parentFolder.AuthorAccountId != _requestAccountIdService.Id)
                    {
                        throw new FunException("Вы не можете перемещать сюда, так как не являетесь владельцем");
                    }
                }
                // else { we just moved the folder to the very root }
            }

            _mapper.Map(updateFolderDto, folder);

            await _folderRepository.Update(folder);
        }

        public async Task<ICollection<FolderWithIdDto>> GetMyRootV1()
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folders = await _folderRepository.GetMany(f => f.ParentId == null && f.AuthorAccountId == requestAccountId);

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        public async Task<ICollection<FolderWithIdDto>> GetSubfoldersByFolderV1(long folderId)
        {
            // TODO: Ensure availability to user
            
            var folders = await _folderRepository.GetMany(f => f.ParentId == folderId);

            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        public async Task MoveToTrashV1(long folderId)
        {
            // TODO: Ensure availability to user
            
            var folder = await _folderRepository.GetById(folderId);

            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                throw new FunException("Необходимо быть владельцем для удаления элемента!");
            }

            folder.IsInTrashBin = true;
            
            await _folderRepository.Update(folder);
        }

        public async Task<ICollection<FolderWithIdDto>> GetMyTrashBinV1()
        {
            var requestAccountId = _requestAccountIdService.Id;
            var folders = await _folderRepository.GetMany(f => f.AuthorAccountId == requestAccountId && f.IsInTrashBin);
            
            var folderWithIdDtos = _mapper.Map<ICollection<FolderWithIdDto>>(folders);

            return folderWithIdDtos;
        }

        public async Task RestoreFromTrashV1(long folderId)
        {
            var folder = await _folderRepository.GetById(folderId);
            
            if (folder.AuthorAccountId != _requestAccountIdService.Id)
            {
                throw new FunException("У вас нет доступа к восстановлению этого элемента!");
            }

            if (!folder.IsInTrashBin)
            {
                throw new FunException("Элемент не в корзине");
            }

            folder.IsInTrashBin = false;

            await _folderRepository.Update(folder);
        }
    }
}