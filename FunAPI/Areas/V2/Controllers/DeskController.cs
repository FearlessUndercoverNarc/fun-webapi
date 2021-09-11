﻿using System;
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
using Services.Versioned.V2;

namespace FunAPI.Areas.V2.Controllers
{
    [Route("/v2/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class DeskController : Controller
    {
        private IDeskServiceV2 _deskService;
        private readonly JsonSerializerSettings _jsonSettings;
        private ISSEService _sseService;

        public DeskController(IDeskServiceV2 deskService, ISSEService sseService)
        {
            _deskService = deskService;
            _sseService = sseService;
            _jsonSettings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
        }
        // Note: JS EventSource doesn't support custom headers, so disable AuthTokenFilter here 
        [HttpGet]
        [MapToApiVersion("1.0")]
        //[TypeFilter(typeof(AuthTokenFilter))]
        public void Sse(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            Response.StatusCode = 200;
            Response.Headers.Add("Content-Type", "text/event-stream");

            async void OnDeskAction(long deskId, long eventId)
            {
                try
                {
                    var messageJson = JsonConvert.SerializeObject(new CreatedDto(id), _jsonSettings);

                    await Response.WriteAsync($"data: {messageJson}\n");
                    await Response.WriteAsync($"id: {id}\n\n"); // NOTE: we have 2 '\n' because of SSE format
                    await Response.Body.FlushAsync();
                }
                catch (Exception e)
                {
                    await TelegramAPI.Send($"/v1/Desk/sse failed in OnDeskAction.\n{e.ToPrettyString()}");
                }
            }

            _sseService.DeskActionOccured += OnDeskAction;

            // _logger.LogInformation("SSE connected");

            string lastEventIdString = Request.Headers["Last-Event-ID"];

            if (int.TryParse(lastEventIdString, out var lastEventId))
            {
                for (var i = lastEventId; i < _sseService.LastDeskActionIdMap[id]; i++)
                {
                    OnDeskAction(id, i);
                }
            }

            HttpContext.RequestAborted.WaitHandle.WaitOne();

            _sseService.DeskActionOccured -= OnDeskAction;

            // _logger.LogInformation("SSE disconnected");
        }
        
        [HttpPost]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult<CreatedDto>> Create(
            [FromBody] CreateDeskDto createDeskDto
        )
        {
            var createdDto = await _deskService.Create(createDeskDto);

            return Ok(createdDto);
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult> Update(
            [FromBody] UpdateDeskDto updateDeskDto
        )
        {
            await _deskService.Update(updateDeskDto);
            return Ok();
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult<ICollection<DeskWithIdDto>>> GetByFolder(
            [Required] [Id(typeof(Folder))] long id
        )
        {
            var deskWithIdDtos = await _deskService.GetByFolder(id);

            return Ok(deskWithIdDtos);
        }
        
        [HttpGet]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult<ICollection<DeskWithIdDto>>> GetSharedToMe()
        {
            var deskWithIdDtos = await _deskService.GetSharedToMe();

            return Ok(deskWithIdDtos);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
        public async Task<ActionResult<ICollection<DeskWithIdDto>>> GetById(
            [Required] [Id(typeof(Desk))] long id
        )
        {
            var deskWithIdDto = await _deskService.GetById(id);

            return Ok(deskWithIdDto);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [TypeFilter(typeof(AuthTokenFilter.WithSubscription))]
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