using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Cards;
using Models.DTOs.Misc;
using Models.Misc;
using Services;
using Services.CommonServices.Abstractions;
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
        private const int MaxCardImageSizeInMegabytes = 5;
        
        private ICardServiceV2 _cardService;

        private IImageService _imageService;

        public CardController(ICardServiceV2 cardService, IImageService imageService)
        {
            _cardService = cardService;
            _imageService = imageService;
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


        [HttpPost]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult<ImageDto>> UploadImage(IFormFile image)
        {
            image.EnsureNotNullHandled("image is missing");

            await using var ms = new MemoryStream();
            await image.CopyToAsync(ms);

            switch (ms.Length)
            {
                case 0:
                    throw new FunException("image was empty");
                case > MaxCardImageSizeInMegabytes * 1024 * 1024:
                    throw new FunException($"Размер изображения превышает максимальный ({MaxCardImageSizeInMegabytes} Мб)");
            }

            ms.Position = 0;

            var imageName = await _imageService.Create(image.FileName, "Cards", ms.ToArray());

            // var file = await _baseImageService.Create(filename, "Uploaded", ms.ToArray());

            return new ImageDto(imageName);
        }
    }
}