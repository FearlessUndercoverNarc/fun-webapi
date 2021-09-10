using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Cards;
using Models.DTOs.Misc;
using Services.Versioned.V2;

namespace FunAPI.Areas.V2.Controllers
{
    [Route("/v2/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class CardController : Controller
    {
        private ICardServiceV2 _cardService;

        public CardController(ICardServiceV2 cardService)
        {
            _cardService = cardService;
        }

        [HttpPost]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<CreatedDto>> Create([FromBody] CreateCardDto createCardDto)
        {
            var createdDto = await _cardService.Create(createCardDto);

            return Ok(createdDto);
        }

        [HttpPost]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> Update([FromBody] UpdateCardDto updateCardDto)
        {
            await _cardService.Update(updateCardDto);

            return Ok();
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ICollection<CardWithIdDto>>> GetAllByDesk(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            var cardWithIdDtos = await _cardService.GetAllByDesk(id);

            return Ok(cardWithIdDtos);
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ICollection<CardWithIdDto>>> GetByDeskAndRect(
            [Required] [Id(typeof(Desk))] long id,
            [Required] [Range(0, 10000)] uint left,
            [Required] [Range(0, 10000)] uint right,
            [Required] [Range(0, 10000)] uint top,
            [Required] [Range(0, 10000)] uint bottom
        )
        {
            var cardWithIdDtos = await _cardService.GetByDeskAndRect(id, left, right, top, bottom);

            return Ok(cardWithIdDtos);
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<CardWithIdDto>> GetById(
            [Required] [Id(typeof(Card))] long id
        )
        {
            var cardWithIdDto = await _cardService.GetById(id);

            return Ok(cardWithIdDto);
        }

        [HttpDelete]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> Remove(
            [Required] [Id(typeof(Card))] long id
        )
        {
            await _cardService.Remove(id);

            return Ok();
        }
    }
}