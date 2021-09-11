using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Db.Tree;
using Models.DTOs;
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
    public class DeskActionController : Controller
    {
        private ISSEService _sseService;
        private IDeskActionServiceV1 _deskActionService;
        private readonly JsonSerializerSettings _jsonSettings;

        public DeskActionController(ISSEService sseService, IDeskActionServiceV1 deskActionService)
        {
            _sseService = sseService;
            _deskActionService = deskActionService;
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
                    var messageJson = JsonConvert.SerializeObject(new CreatedDto(eventId), _jsonSettings);

                    await Response.WriteAsync($"data: {messageJson}\n");
                    await Response.WriteAsync($"id: {eventId}\n\n"); // NOTE: we have 2 '\n' because of SSE format
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
                if (_sseService.LastDeskActionIdMap.ContainsKey(id))
                {
                    for (var i = lastEventId; i < _sseService.LastDeskActionIdMap[id]; i++)
                    {
                        OnDeskAction(id, i);
                    }
                }
            }

            HttpContext.RequestAborted.WaitHandle.WaitOne();

            _sseService.DeskActionOccured -= OnDeskAction;

            // _logger.LogInformation("SSE disconnected");
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<DeskActionDto>> GetById(
            [Required] [Id(typeof(DeskActionHistoryItem))]
            long id
        )
        {
            var deskActionDto = await _deskActionService.GetById(id);

            return Ok(deskActionDto);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<DeskActionDto>> GetAllByDesk(
            [Required] [Id(typeof(Desk))]
            long id
        )
        {
            var deskActionDtos = await _deskActionService.GetAllByDesk(id);

            return Ok(deskActionDtos);
        }
    }
}