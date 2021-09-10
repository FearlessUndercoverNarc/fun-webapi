using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Cards;
using Models.DTOs.Misc;
using Services.Versioned.V1;

namespace FunAPI.Areas.V1.Controllers
{
    [Route("/v1/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    public class CardController : Controller
    {
        private ICardsServiceV1 _cardsService;

        public CardController(ICardsServiceV1 cardsService)
        {
            _cardsService = cardsService;
        }

        [HttpPost]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<CreatedDto>> Create([FromBody] CreateCardDto createCardDto)
        {
            var createdDto = await _cardsService.Create(createCardDto);

            return Ok(createdDto);
        }

        [HttpPost]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> Update([FromBody] UpdateCardDto updateCardDto)
        {
            await _cardsService.Update(updateCardDto);

            return Ok();
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<ICollection<CardWithIdDto>>> GetAllByDesk(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            var cardWithIdDtos = await _cardsService.GetAllByDesk(id);

            return Ok(cardWithIdDtos);
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<ICollection<CardWithIdDto>>> GetByDeskAndRect(
            [Required] [Id(typeof(Desk))] long id,
            [Required] [Range(0, 10000)] uint left,
            [Required] [Range(0, 10000)] uint right,
            [Required] [Range(0, 10000)] uint top,
            [Required] [Range(0, 10000)] uint bottom
        )
        {
            var cardWithIdDtos = await _cardsService.GetByDeskAndRect(id, left, right, top, bottom);

            return Ok(cardWithIdDtos);
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<CardWithIdDto>> GetById(
            [Required] [Id(typeof(Card))] long id
        )
        {
            var cardWithIdDto = await _cardsService.GetById(id);

            return Ok(cardWithIdDto);
        }

        [HttpDelete]
        [TypeFilter(typeof(AuthTokenFilter))]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> Remove(
            [Required] [Id(typeof(Card))] long id
        )
        {
            await _cardsService.Remove(id);

            return Ok();
        }
    }
}