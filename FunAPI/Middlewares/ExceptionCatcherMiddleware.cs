using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.DTOs.Misc;
using Models.Misc;
using Services;
using Services.External;

namespace FunAPI.Middlewares
{
    public class ExceptionCatcherMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionCatcherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (FunException ex)
            {
                await HandleFunExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(context, ex);
            }
        }

        private static async Task HandleFunExceptionAsync(HttpContext context, FunException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorDto(exception.Message));
        }

        private static async Task HandleUnknownExceptionAsync(HttpContext context, Exception exception)
        {
            await TelegramAPI.Send(context.Request.Path + "\n" + exception.ToPrettyString());
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorDto("Извините, произошла непредвиденная ошибка. Разработчики уже уведомлены."));
        }
    }
}