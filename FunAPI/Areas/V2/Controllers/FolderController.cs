using System.Collections.Generic;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Folders;
using Models.DTOs.Misc;
using Services.Versioned.V2;

namespace FunAPI.Areas.V2.Controllers
{
    [Route("/v2/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class FolderController : Controller
    {
        private IFolderServiceV2 _folderServiceV2;

        public FolderController(IFolderServiceV2 folderServiceV2)
        {
            _folderServiceV2 = folderServiceV2;
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<CreatedDto>> Create([FromBody] CreateFolderDto createFolderDto)
        {
            return Ok("Not implemented v2");
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> Update([FromBody] UpdateFolderDto updateFolderDto)
        {
            return Ok("Not implemented v2");
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetMyRoot()
        {
            return Ok("Not implemented v2");
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ICollection<FolderWithIdDto>>> GetSubfoldersByFolder([Id(typeof(Folder))] long id)
        {
            return Ok("Not implemented v2");
        }
    }
}