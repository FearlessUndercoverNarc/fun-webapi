using System.Collections.Generic;
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
            var createdDto = await _folderServiceV1.CreateV1(createFolderDto);

            return Ok(createdDto);
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> Update([FromBody] UpdateFolderDto updateFolderDto)
        {
            await _folderServiceV1.UpdateV1(updateFolderDto);
            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyRoot()
        {
            var folderWithIdDtos = await _folderServiceV1.GetMyRootV1();

            return Ok(folderWithIdDtos);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyTrashBin()
        {
            var folderWithIdDtos = await _folderServiceV1.GetMyTrashBinV1();

            return Ok(folderWithIdDtos);
            ;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToTrashBin([Id(typeof(Folder))] long id)
        {
            await _folderServiceV1.MoveToTrashV1(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RestoreFromTrashBin([Id(typeof(Folder))] long id)
        {
            await _folderServiceV1.RestoreFromTrashV1(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetSubfoldersByFolder([Id(typeof(Folder))] long id)
        {
            var folderWithIdDtos = await _folderServiceV1.GetSubfoldersByFolderV1(id);

            return Ok(folderWithIdDtos);
            ;
        }
    }
}