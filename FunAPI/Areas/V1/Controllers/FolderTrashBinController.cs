using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Services.Versioned.V1;

namespace FunAPI.Areas.V1.Controllers
{
    [Route("/v1/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    public class FolderTrashBinController : Controller
    {
        private IFolderTrashBinServiceV1 _folderTrashBinService;

        public FolderTrashBinController(IFolderTrashBinServiceV1 folderTrashBinService)
        {
            _folderTrashBinService = folderTrashBinService;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyTrashBin()
        {
            var folderWithIdDtos = await _folderTrashBinService.GetMyTrashBin();

            return Ok(folderWithIdDtos);
        }

        [HttpDelete]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _folderTrashBinService.MoveToTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RestoreFromTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _folderTrashBinService.RestoreFromTrashBin(id);

            return Ok();
        }
        
        [HttpDelete]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RemoveFromTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _folderTrashBinService.RemoveFromTrashBin(id);

            return Ok();
        }
    }
}