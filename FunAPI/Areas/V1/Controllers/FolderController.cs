﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Models.DTOs.Misc;
using Services.Versioned.V1;

namespace FunAPI.Areas.V1.Controllers
{
    [Route("/v1/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    public class FolderController : Controller
    {
        private IFolderServiceV1 _folderService;

        public FolderController(IFolderServiceV1 folderService)
        {
            _folderService = folderService;
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<CreatedDto>> Create(
            [FromBody] CreateFolderDto createFolderDto
        )
        {
            var createdDto = await _folderService.Create(createFolderDto);

            return Ok(createdDto);
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> Update(
            [FromBody] UpdateFolderDto updateFolderDto
        )
        {
            await _folderService.Update(updateFolderDto);
            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyRoot()
        {
            var folderWithIdDtos = await _folderService.GetMyRoot();

            return Ok(folderWithIdDtos);
        }
        
        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetSharedToMeRoot()
        {
            var folderWithIdDtos = await _folderService.GetSharedToMeRoot();

            return Ok(folderWithIdDtos);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyTrashBin()
        {
            var folderWithIdDtos = await _folderService.GetMyTrashBin();

            return Ok(folderWithIdDtos);
        }

        [HttpDelete]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _folderService.MoveToTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RestoreFromTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _folderService.RestoreFromTrashBin(id);

            return Ok();
        }
        
        [HttpDelete]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RemoveFromTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _folderService.RemoveFromTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetSubfoldersByFolder(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            var folderWithIdDtos = await _folderService.GetSubfoldersByFolder(id);

            return Ok(folderWithIdDtos);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToFolder(
            [Required] [Id(typeof(Folder))] long id,
            [Id(typeof(Folder))] long? destinationId
        )
        {
            await _folderService.MoveToFolder(id, destinationId);

            return Ok();
        }
    }
}