using System.Collections.Generic;
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
        private IFolderServiceV1 _folderServiceV1;

        public FolderController(IFolderServiceV1 folderServiceV1)
        {
            _folderServiceV1 = folderServiceV1;
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<CreatedDto>> Create([FromBody] CreateFolderDto createFolderDto)
        {
            var createdDto = await _folderServiceV1.Create(createFolderDto);

            return Ok(createdDto);
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> Update([FromBody] UpdateFolderDto updateFolderDto)
        {
            await _folderServiceV1.Update(updateFolderDto);
            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyRoot()
        {
            var folderWithIdDtos = await _folderServiceV1.GetMyRoot();

            return Ok(folderWithIdDtos);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyTrashBin()
        {
            var folderWithIdDtos = await _folderServiceV1.GetMyTrashBin();

            return Ok(folderWithIdDtos);
            ;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToTrashBin(
            [Required] [Id(typeof(Folder))] long id)
        {
            await _folderServiceV1.MoveToTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RestoreFromTrashBin(
            [Required] [Id(typeof(Folder))] long id)
        {
            await _folderServiceV1.RestoreFromTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetSubfoldersByFolder(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            var folderWithIdDtos = await _folderServiceV1.GetSubfoldersByFolder(id);

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
            await _folderServiceV1.MoveToFolder(id, destinationId);

            return Ok();
        }
    }
}