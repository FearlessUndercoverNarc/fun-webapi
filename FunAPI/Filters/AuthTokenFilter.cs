using System;
using System.Threading.Tasks;
using Infrastructure.Verbatims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.DTOs.Misc;
using Services.External;
using Services.SharedServices.Abstractions;

namespace FunAPI.Filters
{
    public class AuthTokenFilter : IAsyncActionFilter
    {
        private readonly ITokenSessionService _tokenSessionService;
        private readonly IRequestAccountIdSetterService _requestAccountIdSetterService;

        public AuthTokenFilter(ITokenSessionService tokenSessionService, IRequestAccountIdSetterService requestAccountIdSetterService)
        {
            _tokenSessionService = tokenSessionService;
            _requestAccountIdSetterService = requestAccountIdSetterService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
#if DEBUG
            // Console.WriteLine("Skipping Auth-Token Check in DEBUG");
            await next.Invoke();
            return;
#else
            // Console.WriteLine("Performing Auth-Token Check in RELEASE");
            if (!context.HttpContext.TryGetAuthToken(out var authToken))
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenMissing));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method without auth-token!");
                return;
            }

            if (string.IsNullOrEmpty(authToken))
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenEmpty));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method with empty auth-token!");
                return;
            }

            if (authToken.Length != 36)
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenInvalidLength));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method with invalid length auth-token!");
                return;
            }

            var accountByToken = await _tokenSessionService.GetAccountByToken(authToken);

            var accountSession = await _tokenSessionService.GetByToken(authToken);
            if (accountSession == null)
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenUnknown));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method with unknown auth-token \"{authToken}\"!");
                return;
            }

            if (accountSession.EndDate < DateTime.Now)
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenExpired));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method with expired auth-token \"{authToken}\"!");
                return;
            }

            
            _requestAccountIdSetterService.Set(accountByToken.Id, accountByToken.HasSubscription);

            await next.Invoke();
#endif
        }
    }
}