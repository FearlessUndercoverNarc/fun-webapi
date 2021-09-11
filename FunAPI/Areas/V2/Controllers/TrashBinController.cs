using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Services.Versioned.V1;
using Services.Versioned.V2;

namespace FunAPI.Areas.V2.Controllers
{
    [Route("/v2/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class TrashBinController : Controller
    {
        private ITrashBinServiceV2 _trashBinService;

        public TrashBinController(ITrashBinServiceV2 trashBinService)
        {
            _trashBinService = trashBinService;
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyTrashBin()
        {
            var folderWithIdDtos = await _trashBinService.GetMyTrashBin();

            return Ok(folderWithIdDtos);
        }

        [HttpDelete]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _trashBinService.MoveToTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RestoreFromTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _trashBinService.RestoreFromTrashBin(id);

            return Ok();
        }
        
        [HttpDelete]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RemoveFromTrashBin(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            await _trashBinService.RemoveFromTrashBin(id);

            return Ok();
        }
    }
}