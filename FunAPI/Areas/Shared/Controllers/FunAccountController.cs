using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.FunAccounts;
using Models.DTOs.Login;
using Models.DTOs.Misc;
using Services.SharedServices.Abstractions;

namespace FunAPI.Areas.Shared.Controllers
{
    [ApiVersionNeutral]
    public class FunAccountController : FunSharedController
    {
        private readonly ITokenSessionService _tokenSessionService;
        private readonly IFunAccountService _funAccountService;

        public FunAccountController(ITokenSessionService tokenSessionService, IFunAccountService funAccountService)
        {
            _tokenSessionService = tokenSessionService;
            _funAccountService = funAccountService;
        }

        [NonAction]
        [HttpGet]
        private ActionResult Test()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < 40; i++)
            {
                builder.Append("<tr>");
                for (int j = 0; j < 80; j++)
                {
                    builder.Append($"<td>{((i * 80 + j) * 2):X}</td>");
                }

                builder.Append("</tr>");
            }

            string table = builder.ToString();

            return Content($@"<!DOCTYPE html>
                   <html lang=""en"">
                <head>
                <meta charset=""UTF-8"">
                <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Up and running</title>
                </head>
                <body>
                <table border=""1"">{table}<table>
                </body>
                </html>", "text/html");
        }

        [HttpPost]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto loginDto)
        {
            var loginResultDto = await _tokenSessionService.Login(loginDto);

            return loginResultDto;
        }

        [HttpPost]
        public async Task<ActionResult<CreatedDto>> Create([FromBody] CreateFunAccountDto createFunAccountDto)
        {
            var createdDto = await _funAccountService.CreateFunAccount(createFunAccountDto);

            return createdDto;
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] UpdateFunAccountDto updateFunAccountDto)
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

        [HttpGet]
        [TypeFilter(typeof(AuthTokenFilter))]
        public async Task<ActionResult<ICollection<FunAccountWithIdDto>>> GetAll()
        {
            var funAccountWithIdDtos = await _funAccountService.GetAll();

            return Ok(funAccountWithIdDtos);
        }
    }
}