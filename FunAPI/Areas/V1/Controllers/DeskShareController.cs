using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Account;
using Models.Db.Tree;
using Models.DTOs.Relations;
using Services.Versioned.V1;

namespace FunAPI.Areas.V1.Controllers
{
    [Route("/v1/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    public class DeskShareController : Controller
    {
        private IDeskShareServiceV1 _deskShareService;

        public DeskShareController(IDeskShareServiceV1 deskShareService)
        {
            _deskShareService = deskShareService;
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> Share(
            [Required] [Id(typeof(Desk))] long id,
            [Required] [Id(typeof(FunAccount))] long recipientId,
            [Required] bool hasWriteAccess
        )
        {
            await _deskShareService.Share(id, recipientId, hasWriteAccess);

            return Ok();
        }
        
        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<ICollection<DeskShareDto>>> GetShares(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            var deskShareDtos = await _deskShareService.GetShares(id);

            return Ok(deskShareDtos);
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> RemoveShare(
            [Required] [Id(typeof(Desk))] long id,
            [Required] [Id(typeof(FunAccount))] long recipientId
        )
        {
            await _deskShareService.RemoveShare(id, recipientId);

            return Ok();
        }
    }
}