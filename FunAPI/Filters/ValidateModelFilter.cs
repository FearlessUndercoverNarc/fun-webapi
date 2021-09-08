using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.DTOs.Misc;
using Services.External;

namespace FunAPI.Filters
{
    public class ValidateModelFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                string message = string.Join("\n",
                    context
                        .ModelState
                        .Select(e => e.Value.Errors)
                        .Where(e => e.Count > 0)
                        .SelectMany(e => e)
                        .Select(e => e.ErrorMessage)
                );
                context.Result = new BadRequestObjectResult(
                    new ErrorDto(message)
                );
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nModel Validation Failed:\n{message}");
                return;
            }

            await next();
        }
    }
}