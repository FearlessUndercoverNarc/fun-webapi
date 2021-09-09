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
        private readonly IRequestAccountIdService _requestAccountIdService;
        private readonly IRequestAccountIdSetterService _requestAccountIdSetterService;

        private readonly ITokenSessionService _tokenSessionService;

        public AuthTokenFilter(ITokenSessionService tokenSessionService, IRequestAccountIdSetterService requestAccountIdSetterService, IRequestAccountIdService requestAccountIdService)
        {
            _tokenSessionService = tokenSessionService;
            _requestAccountIdSetterService = requestAccountIdSetterService;
            _requestAccountIdService = requestAccountIdService;
        }

        public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (await Validate(context, next)) await next.Invoke();
        }

        private async Task<bool> Validate(ActionExecutingContext context, ActionExecutionDelegate next)
        {
#if DEBUG
            // Console.WriteLine("Skipping Auth-Token Check in DEBUG");
            return true;
#else
            // Console.WriteLine("Performing Auth-Token Check in RELEASE");
            if (!context.HttpContext.TryGetAuthToken(out var authToken))
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenMissing));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method without auth-token!");
                return false;
            }

            if (string.IsNullOrEmpty(authToken))
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenEmpty));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method with empty auth-token!");
                return false;
            }

            if (authToken.Length != 36)
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenInvalidLength));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method with invalid length auth-token!");
                return false;
            }

            var accountByToken = await _tokenSessionService.GetAccountByToken(authToken);

            var accountSession = await _tokenSessionService.GetByToken(authToken);
            if (accountSession == null)
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenUnknown));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method with unknown auth-token \"{authToken}\"!");
                return false;
            }

            if (accountSession.EndDate < DateTime.Now)
            {
                context.Result = new UnauthorizedObjectResult(new ErrorDto(VMessages.AuthTokenExpired));
                await TelegramAPI.Send($"{context.HttpContext.Request.Path.ToString()}\nAttempt to access auth-token protected method with expired auth-token \"{authToken}\"!");
                return false;
            }

            _requestAccountIdSetterService.Set(accountByToken.Id, accountByToken.HasSubscription);
            return true;
#endif
        }

        public class WithSubscription : AuthTokenFilter
        {
            public WithSubscription(ITokenSessionService tokenSessionService, IRequestAccountIdSetterService requestAccountIdSetterService, IRequestAccountIdService requestAccountIdService) : base(tokenSessionService, requestAccountIdSetterService, requestAccountIdService)
            {
            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (!await Validate(context, next)) return;

                if (!_requestAccountIdService.HasSubscription)
                {
                    context.Result = new UnauthorizedObjectResult(new ErrorDto("This method requires subscription"));
                    return;
                }

                await next();
            }
        }
    }
}