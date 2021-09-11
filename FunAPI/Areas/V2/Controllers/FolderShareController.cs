using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Account;
using Models.Db.Tree;
using Services.Versioned.V2;

namespace FunAPI.Areas.V2.Controllers
{
    [Route("/v2/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class FolderShareController : Controller
    {
        private IFolderShareServiceV2 _folderShareService;

        public FolderShareController(IFolderShareServiceV2 folderShareService)
        {
            _folderShareService = folderShareService;
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> Share(
            [Required] [Id(typeof(Folder))] long id,
            [Required] [Id(typeof(FunAccount))] long recipientId
        )
        {
            await _folderShareService.Share(id, recipientId);

            return Ok();
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> RemoveShare(
            [Required] [Id(typeof(Folder))] long id,
            [Required] [Id(typeof(FunAccount))] long recipientId
        )
        {
            await _folderShareService.RemoveShare(id, recipientId);

            return Ok();
        }
    }
}