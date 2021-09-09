using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.FunAccounts;
using Models.DTOs.Login;
using Models.DTOs.Misc;
using Services.SharedServices.Abstractions;

namespace FunAPI.Areas.Shared.Controllers
{
    public class FunAccountController : FunSharedController
    {
        private readonly ITokenSessionService _tokenSessionService;
        private readonly IFunAccountService _funAccountService;

        public FunAccountController(ITokenSessionService tokenSessionService, IFunAccountService funAccountService)
        {
            _tokenSessionService = tokenSessionService;
            _funAccountService = funAccountService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto loginDto)
        {
            var loginResultDto = await _tokenSessionService.Login(loginDto);

            return loginResultDto;
        }

        [HttpPost]
        public async Task<ActionResult<CreatedDto>> CreateAccount([FromBody] CreateFunAccountDto createFunAccountDto)
        {
            var createdDto = await _funAccountService.CreateFunAccount(createFunAccountDto);

            return createdDto;
        }

        [HttpPost]
        public async Task<ActionResult> UpdateAccount([FromBody] UpdateFunAccountDto updateFunAccountDto)
        {
            await _funAccountService.UpdateFunAccount(updateFunAccountDto);

            return Ok();
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