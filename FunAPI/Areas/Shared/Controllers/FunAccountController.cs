using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Misc;
using Models.DTOs.Requests;
using Services.SharedServices.Abstractions;

namespace FunAPI.Areas.Shared.Controllers
{
    public class FunAccountController : FunSharedController
    {
        private readonly ITokenSessionService _tokenSessionService;

        public FunAccountController(ITokenSessionService tokenSessionService)
        {
            _tokenSessionService = tokenSessionService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto loginDto)
        {
            var loginResultDto = await _tokenSessionService.Login(loginDto);

            return loginResultDto;
        }

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<MessageDto>> Logout()
        {
            ControllerContext.HttpContext.TryGetAuthToken(out var token);

            await _tokenSessionService.Logout(token);
            
            return Ok();
        }
    }
}