using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Desks;
using Models.DTOs.Misc;
using Services.Versioned.V2;

namespace FunAPI.Areas.V2.Controllers
{
    [Route("/v2/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    public class DeskController : Controller
    {
        private IDeskServiceV2 _deskService;

        public DeskController(IDeskServiceV2 deskService)
        {
            _deskService = deskService;
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<CreatedDto>> Create([FromBody] CreateDeskDto createDeskDto)
        {
            var createdDto = await _deskService.Create(createDeskDto);

            return Ok(createdDto);
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> Update([FromBody] UpdateDeskDto updateDeskDto)
        {
            await _deskService.Update(updateDeskDto);
            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToTrashBin(
            [Required] [Id(typeof(Desk))] long id)
        {
            await _deskService.MoveToTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> RestoreFromTrashBin(
            [Required] [Id(typeof(Desk))] long id)
        {
            await _deskService.RestoreFromTrashBin(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToFolder(
            [Required] [Id(typeof(Desk))] long id,
            [Id(typeof(Folder))] long? destinationId
        )
        {
            await _deskService.MoveToFolder(id, destinationId);

            return Ok();
        }
    }
}