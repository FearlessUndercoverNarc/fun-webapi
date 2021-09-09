using System.Threading.Tasks;
using Infrastructure.Verbatims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.DTOs.Misc;
using Services.External;

namespace FunAPI.Filters
{
    public class SudoFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Query.TryGetValue("sudo", out var sudo))
            {
                if (sudo == "egop")
                {
                    await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nSudo protected method access succeeded!");
                    await next();
                }
                else
                {
                    context.Result = new BadRequestObjectResult(new ErrorDto(VMessages.InvalidSudoKey));
                    await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nInvalid sudo key \"{sudo}\"!");
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult(new ErrorDto(VMessages.SudoAccessRequired));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access sudo protected method!");
            }
        }
    }
}