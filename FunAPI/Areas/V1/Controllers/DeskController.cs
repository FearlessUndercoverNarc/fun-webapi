using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs.Desks;
using Models.DTOs.Misc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Services;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace FunAPI.Areas.V1.Controllers
{
    [Route("/v1/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    public class DeskController : Controller
    {
        private IDeskServiceV1 _deskService;

        public DeskController(IDeskServiceV1 deskService, ISSEService sseService)
        {
            _deskService = deskService;
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<CreatedDto>> Create(
            [FromBody] CreateDeskDto createDeskDto
        )
        {
            var createdDto = await _deskService.Create(createDeskDto);

            return Ok(createdDto);
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> Update(
            [FromBody] UpdateDeskDto updateDeskDto
        )
        {
            await _deskService.Update(updateDeskDto);
            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<DeskWithIdDto>>> GetByFolder(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            var deskWithIdDtos = await _deskService.GetByFolder(id);

            return Ok(deskWithIdDtos);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<DeskWithIdDto>>> GetSharedToMe()
        {
            var deskWithIdDtos = await _deskService.GetSharedToMe();

            return Ok(deskWithIdDtos);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<DeskWithIdDto>>> GetById(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            var deskWithIdDto = await _deskService.GetById(id);

            return Ok(deskWithIdDto);
        }


        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult> MoveToFolder(
            [Required] [Id(typeof(Desk))] long id,
            [Required] [Id(typeof(Folder))] long destinationId
        )
        {
            await _deskService.MoveToFolder(id, destinationId);

            return Ok();
        }
    }
}