using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Desks;
using Services.Versioned.V1;

namespace FunAPI.Areas.V1.Controllers
{
    [Route("/v1/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    public class DeskTrashBinController : Controller
    {
        private IDeskTrashBinServiceV1 _deskTrashBinService;

        public DeskTrashBinController(IDeskTrashBinServiceV1 deskTrashBinService)
        {
            _deskTrashBinService = deskTrashBinService;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<DeskWithIdDto>>> GetMyTrashBin()
        {
            var deskWithIdDtos = await _deskTrashBinService.GetMyTrashBin();

            return Ok(deskWithIdDtos);
        }

        [HttpDelete]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToTrashBin(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            await _deskTrashBinService.MoveToTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RestoreFromTrashBin(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            await _deskTrashBinService.RestoreFromTrashBin(id);

            return Ok();
        }

        [HttpDelete]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RemoveFromTrashBin(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            await _deskTrashBinService.RemoveFromTrashBin(id);

            return Ok();
        }
    }
}