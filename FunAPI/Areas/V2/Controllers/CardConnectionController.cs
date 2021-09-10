using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.CardConnections;
using Models.DTOs.Misc;
using Services.Versioned.V2;

namespace FunAPI.Areas.V2.Controllers
{
    [Route("/v2/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class CardConnectionController : Controller
    {
        private readonly ICardConnectionServiceV2 _cardConnectionService;

        public CardConnectionController(ICardConnectionServiceV2 cardConnectionService)
        {
            _cardConnectionService = cardConnectionService;
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult<CreatedDto>> Create(
            [FromBody] CreateCardConnectionDto createCardConnectionDto
        )
        {
            var createdDto = await _cardConnectionService.Create(createCardConnectionDto);

            return Ok(createdDto);
        }

        [HttpDelete]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult> Remove(
            [Required] [Id(typeof(CardConnection))]
            long id
        )
        {
            await _cardConnectionService.Remove(id);

            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult<ICollection<CardConnectionWithIdDto>>> GetAllByDesk(
            [Required] [Id(typeof(Desk))]
            long id
        )
        {
            var cardConnectionWithIdDtos = await _cardConnectionService.GetAllByDesk(id);

            return Ok(cardConnectionWithIdDtos);
        }
    }
}